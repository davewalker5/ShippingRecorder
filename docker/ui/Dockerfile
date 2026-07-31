FROM mcr.microsoft.com/dotnet/aspnet:latest AS runtime
ARG VERSION
COPY shippingrecorder.mvc-${VERSION}.0 /opt/shippingrecorder.mvc
WORKDIR /opt/shippingrecorder.mvc/bin
ENTRYPOINT [ "./ShippingRecorder.Mvc" ]
