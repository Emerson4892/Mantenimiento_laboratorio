\# Sistema de Gestión de Mantenimiento de Equipos de Laboratorio



\## 1. Descripción del proyecto



El Sistema de Gestión de Mantenimiento de Equipos de Laboratorio es una aplicación web desarrollada para administrar equipos, registrar fallas, asignar técnicos, finalizar mantenimientos y consultar reportes.



El sistema integra una interfaz web desarrollada con Blazor, una API REST en ASP.NET Core, una base de datos PostgreSQL y workflows automatizados mediante n8n.



El proyecto fue desarrollado individualmente como parte del curso Análisis de Sistemas II de la Universidad Mariano Gálvez de Guatemala.



\## 2. Objetivo general



Desarrollar un sistema web que permita automatizar la gestión del mantenimiento de equipos de laboratorio mediante la integración de tecnologías web, bases de datos y flujos de trabajo automatizados.



\## 3. Funcionalidades principales



El sistema permite realizar las siguientes operaciones:



\- Registrar equipos de laboratorio.

\- Reportar fallas y establecer prioridades.

\- Asignar mantenimientos a técnicos responsables.

\- Registrar las soluciones aplicadas.

\- Finalizar mantenimientos y actualizar estados.

\- Consultar reportes de equipos y reparaciones.

\- Buscar registros y navegar entre páginas de resultados.

\- Iniciar y cerrar sesión.

\- Registrar operaciones de auditoría.



\## 4. Tecnologías utilizadas



| Tecnología | Función |

|---|---|

| C# | Lenguaje de programación |

| .NET 10 | Plataforma de desarrollo |

| ASP.NET Core Web API | Servicios HTTP y lógica de integración |

| Blazor Web App | Interfaz gráfica |

| PostgreSQL | Persistencia de información |

| n8n | Automatización de procesos |

| Docker | Ejecución mediante contenedores |

| Bootstrap | Diseño de la interfaz |

| Git y GitHub | Control de versiones |



\## 5. Arquitectura del sistema



La solución utiliza una arquitectura distribuida en componentes.



El usuario interactúa con la aplicación Blazor, que se comunica con la API ASP.NET Core. La API procesa las solicitudes y utiliza n8n para ejecutar las automatizaciones.



Los workflows de n8n realizan operaciones sobre PostgreSQL y devuelven los resultados correspondientes.



También se utilizan consultas desde la API hacia PostgreSQL para obtener las listas de fallas pendientes y mantenimientos asignados.



```text

&#x20;               USUARIO

&#x20;                  |

&#x20;                  v

&#x20;            BLAZOR WEB

&#x20;             Puerto 8081

&#x20;                  |

&#x20;                  v

&#x20;           ASP.NET CORE API

&#x20;             Puerto 8080

&#x20;               /       \\

&#x20;              v         v

&#x20;            n8n      PostgreSQL

&#x20;         Puerto 5678  Puerto 5433\*

&#x20;              |

&#x20;              v

&#x20;          PostgreSQL

```



\*El puerto 5433 corresponde al acceso desde Windows; PostgreSQL utiliza el puerto 5432 dentro del contenedor.\*



\## 6. Estructura del proyecto



```text

MantenimientoLaboratorio/

|

|-- Mantenimiento.Api/

|   |-- Controllers/

|   |-- Dockerfile

|   |-- Program.cs

|   |-- Mantenimiento.Api.csproj

|

|-- Mantenimiento.Web/

|   |-- Components/

|   |   |-- Layout/

|   |   |-- Pages/

|   |   |-- App.razor

|   |   |-- Routes.razor

|   |-- wwwroot/

|   |-- Dockerfile

|   |-- Program.cs

|   |-- Mantenimiento.Web.csproj

|

|-- Mantenimiento.Application/

|   |-- DTOs/

|   |-- Interfaces/

|

|-- Mantenimiento.Domain/

|   |-- Entities/

|

|-- Mantenimiento.Infrastructure/

|   |-- Services/

|

|-- workflows/

|

|-- docs/

|

|-- MantenimientoLaboratorio.slnx

|-- .gitignore

|-- README.md

```



\## 7. Requisitos para ejecutar el proyecto



Para ejecutar la solución se necesita:



\- Docker Desktop.

\- Git.

\- .NET SDK 10, si se desea compilar fuera de Docker.

\- PostgreSQL.

\- n8n.

\- Un navegador web.



El proyecto fue desarrollado y probado en Windows utilizando Docker Desktop.



\## 8. Instalación del proyecto



\### 8.1. Clonar el repositorio



```bash

git clone https://github.com/TU\_USUARIO/MantenimientoLaboratorio.git

```



Ingresar al directorio:



```bash

cd MantenimientoLaboratorio

```



Reemplazar `TU\_USUARIO` por el usuario correspondiente de GitHub.



\### 8.2. Restaurar dependencias



Los paquetes NuGet se encuentran declarados en los archivos `.csproj`.



Para restaurar las dependencias:



```bash

dotnet restore MantenimientoLaboratorio.slnx

```



Para compilar:



```bash

dotnet build MantenimientoLaboratorio.slnx

```



También es posible realizar la compilación utilizando el SDK de .NET 10 dentro de Docker.



\### 8.3. Preparar PostgreSQL



La aplicación utiliza una base de datos denominada:



```text

mantenimiento\_db

```



Las principales tablas del sistema son:



\- `equipos`

\- `fallas`

\- `mantenimientos`

\- `auditoria`



La estructura de la base de datos debe restaurarse antes de ejecutar la aplicación.



El esquema SQL se encuentra en:



```text

docs/mantenimiento\_schema.sql

```



La conexión a PostgreSQL se configura mediante variables de entorno.



\### 8.4. Preparar n8n



Es necesario iniciar una instancia local de n8n y configurar su conexión con PostgreSQL.



Los workflows se encuentran en la carpeta:



```text

workflows/

```



Deben importarse en n8n, configurar sus credenciales y activar los webhooks correspondientes.



\## 9. Workflows de automatización



\### WF01. Registrar equipo



Recibe la información del equipo, valida los datos y registra la información en PostgreSQL.



\### WF02. Reportar falla



Permite registrar incidencias relacionadas con los equipos de laboratorio.



Incluye validaciones para comprobar la información recibida y la existencia del equipo.



\### WF03. Asignar mantenimiento



Recibe el identificador de una falla y los datos del técnico responsable.



Verifica la disponibilidad de la falla y registra la asignación del mantenimiento.



Se incorporó una restricción en PostgreSQL para prevenir dos mantenimientos activos simultáneos sobre una misma falla.



\### WF04. Finalizar mantenimiento



Permite registrar la solución aplicada y actualizar los estados relacionados con el mantenimiento, la falla y el equipo.



\### WF06. Reporte de mantenimiento



Consulta la información almacenada en PostgreSQL y genera los resultados que se presentan en el módulo de reportes de Blazor.



\## 10. Ejecución mediante Docker



Las imágenes de la aplicación son:



```text

emerson0134/mantenimiento-api:1.0

emerson0134/mantenimiento-web:1.0

```



Las versiones disponibles en Docker Hub deben corresponder al código publicado en el repositorio.



\### 10.1. Crear la red Docker



```powershell

docker network create mantenimiento-dev-net

```



Si la red ya existe, no es necesario volver a crearla.



\### 10.2. Iniciar los servicios de datos



En el entorno de desarrollo utilizado, PostgreSQL y n8n se ejecutan en contenedores llamados:



```text

postgres

n8n

```



Para iniciarlos, cuando ya están creados:



```powershell

docker start postgres n8n

```



\### 10.3. Configurar la API



La API necesita las siguientes variables de entorno:



```text

N8n\_\_BaseUrl

N8n\_\_ApiKey

ConnectionStrings\_\_MantenimientoDb

```



Las credenciales deben proporcionarse al ejecutar el contenedor y nunca almacenarse directamente en el repositorio.



\### 10.4. Configurar Blazor



La aplicación web utiliza:



```text

Api\_\_BaseUrl

ASPNETCORE\_ENVIRONMENT

```



El módulo de autenticación utiliza variables de configuración para las credenciales de acceso.



\### 10.5. Iniciar los contenedores existentes



En una instalación que ya tenga los contenedores configurados:



```powershell

docker start postgres n8n mantenimiento-api-app mantenimiento-web-app

```



Este comando no crea una instalación nueva. Solo inicia contenedores previamente configurados.



\## 11. Acceso a la aplicación



Cuando los contenedores estén ejecutándose, utilizar las siguientes direcciones:



| Servicio | Dirección |

|---|---|

| Aplicación web | http://localhost:8081 |

| API REST | http://localhost:8080 |

| n8n | http://localhost:5678 |

| PostgreSQL | localhost:5433 |



La aplicación web dispone de una pantalla de inicio de sesión que permite acceder a los módulos protegidos.



\## 12. Seguridad



Durante el desarrollo se implementaron medidas básicas de seguridad:



\- Configuración de credenciales mediante variables de entorno.

\- Validación de datos recibidos.

\- Autenticación mediante cookies en Blazor.

\- Restricción de acceso a páginas protegidas.

\- Uso de tokens antifalsificación en los formularios de autenticación.

\- Registro de operaciones de auditoría.

\- Restricción para evitar mantenimientos activos duplicados sobre una misma falla.



La protección de las páginas web no sustituye la autorización independiente de los endpoints de la API.



\## 13. Pruebas realizadas



Se realizaron pruebas funcionales de los principales procesos del sistema.



| Prueba | Resultado observado |

|---|---|

| Registro de equipos | Funcionamiento comprobado |

| Reporte de fallas | Funcionamiento comprobado |

| Asignación de mantenimiento | Registro comprobado |

| Finalización de mantenimiento | Funcionamiento comprobado |

| Consulta de reportes | Funcionamiento comprobado |

| Consulta de fallas pendientes | Funcionamiento comprobado |

| Consulta de mantenimientos asignados | Funcionamiento comprobado |

| Inicio de sesión | Funcionamiento comprobado |

| Restricción de asignaciones activas duplicadas | Índice único creado en PostgreSQL |



Las evidencias gráficas y técnicas se encuentran en la documentación del proyecto.



\## 14. Manejo de errores



El sistema utiliza validaciones en los formularios y en los workflows para detectar solicitudes incompletas o incorrectas.



También se incorporaron respuestas de rechazo en el proceso de asignación de mantenimiento.



La configuración del workflow independiente de `Error Trigger` y su registro de errores debe verificarse en las exportaciones finales de n8n.



\## 15. Limitaciones y mejoras futuras



Entre las mejoras previstas se encuentran:



\- Fortalecer la autorización de los endpoints de la API.

\- Administrar usuarios y contraseñas mediante almacenamiento seguro.

\- Completar la protección transaccional de las operaciones críticas.

\- Mejorar las estadísticas del Dashboard.

\- Automatizar el despliegue completo mediante Docker Compose.

\- Ampliar las pruebas de integración y recuperación ante errores.



\## 16. Conclusiones



El desarrollo permitió integrar diferentes tecnologías para automatizar el mantenimiento de equipos de laboratorio.



Mediante n8n se implementaron workflows capaces de procesar solicitudes y ejecutar operaciones sobre PostgreSQL.



La aplicación Blazor proporciona una interfaz gráfica para administrar equipos, registrar fallas, asignar mantenimientos y consultar reportes.



La utilización de Docker facilitó la ejecución local de los componentes del sistema.



El proyecto permitió aplicar conocimientos de análisis de sistemas, desarrollo web, integración de servicios, bases de datos y automatización de procesos.



\## 17. Autor



\*\*Proyecto:\*\* Gestión de Mantenimiento de Equipos de Laboratorio.



\*\*Universidad:\*\* Universidad Mariano Gálvez de Guatemala.



\*\*Curso:\*\* Análisis de Sistemas II.



\*\*Modalidad:\*\* Individual.



\*\*Estudiante:\*\* \[Nombre completo].



\*\*Carné:\*\* \[Número de carné].

#   M a n t e n i m i e n t o _ l a b o r a t o r i o  
 