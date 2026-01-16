FROM mcr.microsoft.com/dotnet/core/aspnet:latest
COPY shippingrecorder.api-1.8.0.0 /opt/shippingrecorder.api
WORKDIR /opt/shippingrecorder.api/bin
ENTRYPOINT [ "./ShippingRecorder.Api" ]
