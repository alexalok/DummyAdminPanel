# How to run

1. Build Docker image

```sh
docker build -f DummyAdminPanel/Dockerfile -t dummyadminpanel .
docker run --rm -it -p 5000:80 dummyadminpanel 
```

2. Navigate to http://localhost:5000