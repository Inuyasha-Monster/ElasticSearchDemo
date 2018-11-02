using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearchDemo.ElasticSearchResponseModel;
using ElasticSearchDemo.RequestModel;
using ElasticSearchDemo.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nest;
using Newtonsoft.Json;

namespace ElasticSearchDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticSearchController : ControllerBase
    {
        private readonly ElasticClient _client;

        public ElasticSearchController(IEsClientProvider clientProvider)
        {
            _client = clientProvider.GetClient();
        }

        [HttpGet]
        [Route("book/novel/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new NotFoundResult();
            }
            var response = await _client.GetAsync<Novel>(new GetRequest("book", "noval", new Id(id)));
            if (response == null || !response.Found)
            {
                return new NotFoundResult();
            }
            return new JsonResult(response.Source);
        }

        [HttpPost]
        [Route("add/book/novel")]
        public async Task<IActionResult> Add([FromForm]NovelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            var response = await _client.IndexAsync<Novel>(new IndexRequest<Novel>(new DocumentPath<Novel>(new Novel()
            {
                Author = request.Author,
                PublishDate = request.PublishDate,
                Title = request.Title,
                WordCount = request.WordCount
            })));
            if (response == null || !response.IsValid)
            {
                return new BadRequestObjectResult(response?.ServerError);
            }
            return new OkObjectResult(response.Id);
        }

        [HttpDelete]
        [Route("delete/book/novel")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var response = await _client.DeleteAsync(new DeleteRequest("book", "noval", id));
            if (!response.IsValid)
            {
                return new NotFoundResult();
            }

            return Ok(response.Id);
        }

        [HttpPut]
        [Route("update/book/novel")]
        public async Task<IActionResult> Update([BindRequired, FromForm]string id, [BindRequired, FromForm]string title, [BindRequired, FromForm]string author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updateResponse = await _client.UpdateAsync<Novel, object>(id, x => x.Doc(new { Title = title, Author = author }).RetryOnConflict(3));

            if (!updateResponse.IsValid)
            {
                return BadRequest(updateResponse.ServerError);
            }

            return Ok(updateResponse.Id);
        }

        [HttpPost]
        [Route("query/book/novel")]
        public async Task<IActionResult> Query([FromForm]string title, [FromForm] string author, [FromForm]int minWordCount, [FromForm]int maxWordCount, [FromForm, JsonConverter(typeof(DateTimeFormatConverter), "yyyy-MM-dd")] DateTime minPublishDate, [FromForm, JsonConverter(typeof(DateTimeFormatConverter), "yyyy-MM-dd")] DateTime maxPublishDate)
        {
            if (maxWordCount <= 0) maxWordCount = int.MaxValue;
            if (minPublishDate == DateTime.MinValue) minPublishDate = new DateTime(1900, 1, 1);
            if (maxPublishDate == DateTime.MinValue) maxPublishDate = DateTime.Now.Date;
            var searchResponse = await _client.SearchAsync<Novel>(x => x
                .Query(y => y
                    .Bool(m => m
                        .Must(o =>
                            o.Match(n => n
                                .Field(f => f.Title)
                                .Query(title)
                            ),
                            p => p.Match(l => l
                                .Field(f => f.Author)
                                .Query(author)
                            ),
                            v => v
                            .Bool(b => b
                                .Filter(q => q
                                    .Range(f => f
                                        .Field(n => n.WordCount)
                                        .GreaterThan(minWordCount)
                                        .LessThanOrEquals(maxWordCount)
                                    )
                                )
                            ),
                            v => v.Bool(b => b
                                .Filter(q => q
                                    .DateRange(f => f
                                        .Field(n => n.PublishDate)
                                        .GreaterThan(DateMath.FromString(minPublishDate.ToString("yyyy-MM-dd")))
                                        .LessThanOrEquals(DateMath.Anchored(maxPublishDate.ToString("yyyy-MM-dd")))
                                    )
                                )
                            )
                        )
                    )
                )
                .From(0).Size(100).Sort(c => c.Descending(v => v.PublishDate)));

            if (!searchResponse.IsValid)
            {
                return BadRequest(searchResponse.ServerError);
            }

            return new JsonResult(searchResponse.Documents);
        }
    }
}