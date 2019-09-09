# docker build -f build/build-test.dockerfile -t mongo-test .

FROM mcr.microsoft.com/dotnet/core/sdk:2.2
ENV TZ="Asia/Taipei"
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
WORKDIR /src
COPY . .
ENV MONGO_CONN="mongodb://mongo:27017"
ENV LOG_PATH="/tmp/test.log"
ENTRYPOINT dotnet test && \
    cat "/tmp/test.log"