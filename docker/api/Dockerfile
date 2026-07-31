FROM mcr.microsoft.com/dotnet/core/aspnet:latest
ARG VERSION
COPY shippingrecorder.api-${VERSION}.0 /opt/shippingrecorder.api
WORKDIR /opt/shippingrecorder.api/bin
ENTRYPOINT [ "./ShippingRecorder.Api" ]
