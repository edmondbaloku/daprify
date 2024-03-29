namespace Daprify.Templates
{
  public class PubSubTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: rabbitmq
spec:
  type: pubsub.rabbitmq
  version: v1
  metadata:
    - name: connectionString
      value: amqp://rabbitmq:5672 # Required. Example: ""amqp://rabbitmq.default.svc.cluster.local:5672"", ""amqp://localhost:5672""
    # - name: durable
    #   value: <REPLACE-WITH-DURABLE> # Optional. Default: ""false""
    # - name: deletedWhenUnused
    #   value: <REPLACE-WITH-DELETE-WHEN-UNUSED> # Optional. Default: ""false""
    # - name: autoAck
    #   value: <REPLACE-WITH-AUTO-ACK> # Optional. Default: ""false""
    # - name: deliveryMode
    #   value: <REPLACE-WITH-DELIVERY-MODE> # Optional. Default: ""0"". Values between 0 - 2.
    # - name: requeueInFailure
    #   value: <REPLACE-WITH-REQUEUE-IN-FAILURE> # Optional. Default: ""false"".
  
# Other brokers and settings can also be used, for more information: https://docs.dapr.io/reference/components-reference/supported-pubsub/
";
  }
}