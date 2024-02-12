Procesador envios

Este es un trabajo realizado en la facultad para la materia de Integración de aplicaciones en entorno web

Esta API se encargada de recibir pedidos de envío y asignar la entrega del mismo a un operador específico según el area de cobertura y la ubicación del cliente.
Permite también:
  - Manejar el estado de los pedidos
  - Suscribir nuevos operadores para luego ser notificados mediante webhooks

Además la API esta securizada mediante Auth0 y fue desplegada en AWS utilizando Docker y AWS Fargate

Las tecnologías utilizadas fueron: C#, .NET 6.0, EntityFramework, SQLServer, Docker, AWS Fargate
