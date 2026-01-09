FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
COPY shippingrecorder.mvc-1.5.0.0 /opt/shippingrecorder.mvc
WORKDIR /opt/shippingrecorder.mvc/bin
ENTRYPOINT [ "./ShippingRecorder.Mvc" ]
