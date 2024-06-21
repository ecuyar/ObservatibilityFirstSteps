# ObservatibilityFirstSteps

ObservatibilityFirstSteps is a really simple console program to show how to implement observability functionality to our program. It has comments for key points. I didn't push to master in one commit to preserve the history. 
(Projects shouldn't be pushed in one commit. - note to myself.) 

In this project [Docker](https://www.docker.com/), [OpenTelemetry](https://opentelemetry.io/), [Jaeger](https://www.jaegertracing.io/) and [Zipkin](https://zipkin.io/) (to see other tracing platforms) are used.

Jaeger and Zipkin run on their default ports. You can change it on `docker-compose.yml` file or `Program.cs` file.

## Usage
- Install docker and using `docker compose up` command create and start container.
- Run the project and see trace data on Jaeger/Zipkin. In order to use Zipkin, uncomment the `.AddZipkinExporter()` method in `Program.cs`
