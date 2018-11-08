### ElasticSearch学习笔记

```json
GET _search
{
  "query": {
    "match_all": {}
  }
}
```

```json
GET _cat/indices
```

```json
GET _search
{
  "query": {
    "match": {
      "title": "ElasticSearch 入门"
    }
  }
}
```

```json
GET _search
{
  "query": {
    "match_phrase": {
      "title": "ES"
    }
  }
}
```

```json
GET book/noval/AWbOAr8ZbSDWqAKH2SfP
```

```json
POST _analyze
{
  "analyzer": "standard",
  "text": "hello world"
}
```

```json
POST book/_analyze
{
  "field": "title",
  "text": "hello world"
}
```

```json
POST _analyze
{
  "tokenizer": "standard",
  "filter": [
    "uppercase"
  ],
  "text": "hello world"
}
```

```json
GET _analyze
{
  "tokenizer": "path_hierarchy",
  "text": "/one/two/three"
}
```

```json
PUT my_index
{
  "mappings": {
    "doc": {
      "dynamic":"strict",
      "properties": {
        "title":{
          "type": "text"
        },
        "name":{
          "type": "keyword"
        },
        "age":{
          "type": "integer"
        }
      }
    }
  }
}
```

```json
GET _analyze
{
  "tokenizer": "keyword",
  "char_filter": [
    "html_strip"
  ],
  "text": "<p>i am so <b>happy</b></p>"
}
```

```json
DELETE my_index
```

```json
GET my_index/_mappings
```

```json
PUT my_index/doc/1
{
  "title":"hello world",
  "desc":"i am djlnet"
}
```

```json
GET my_index/doc/1
```

```json
GET my_index/doc/_search
{
  "query": {
    "match": {
      "desc": "am"
    }
  }
}
```

```json
PUT test_copy_to_index
{
  "mappings": {
    "doc":{
      "properties": {
        "first_name":{
          "type": "keyword",
          "copy_to": "full_name"
        },
         "last_name":{
          "type": "keyword",
          "copy_to": "full_name"
        }
      }
    }
  }
}
```

```json
PUT test_copy_to_index/doc/1
{
  "first_name":"John",
  "last_name":"Smith"
}
```

```json
GET test_copy_to_index/_search
{
  "query": {
    "match": {
      "full_name": {
        "query": "John Smith",
        "operator": "and"
      }
    }
  }
}
```

```json
DELETE test_dynamic_template_index
```

```json
PUT test_dynamic_template_index
{
  "mappings": {
    "doc":{
      "dynamic_templates":[
        {
          "strings_as_keyword":{
            "match_mapping_type":"string",
            "mapping":{
              "type":"keyword"
            }
          }
        },
        {
          "message_as_text":{
            "match_mapping_type":"string",
            "match":"message",
            "mapping":{
              "type":"text"
            }
          }
        }
        ]
    }
  }  
}
```

```json
PUT test_dynamic_template_index/doc/1
{
  "name":"test",
  "message":"i am fine , thx"
}
```

```json
GET test_dynamic_template_index/_mappings
```

```json
PUT _template/test_template
{
  "template": "fo*", 
  "order":0,
  "settings": {
    "number_of_shards": 1,
    "number_of_replicas": 1
  },
  "mappings": {
    "doc": {
      "_source": {
        "enabled": false
      },
      "properties": {
        "name":{
          "type": "keyword"
        }
      }
    }
  }
}
```

```json
PUT _template/test_template2
{
  "template": "food*", 
  "order":1,
  "settings": {
    "number_of_shards": 2,
    "number_of_replicas": 1
  },
  "mappings": {
    "doc": {
      "_source": {
        "enabled": true
      },
      "properties": {
        "name":{
          "type": "keyword"
        }
      }
    }
  }
}
```

```json

GET _template

PUT fo_test

GET fo_test/_mappings

DELETE food_fuck

PUT food_fuck


GET food_fuck/_mapping
GET food_fuck/_settings
```