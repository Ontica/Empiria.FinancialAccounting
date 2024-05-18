# Empiria Financial Accounting System

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3eb36cbce7564607855c8995a3796d77)](https://www.codacy.com/gh/Ontica/Empiria.FinancialAccounting/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Ontica/Empiria.FinancialAccounting&amp;utm_campaign=Badge_Grade)
&nbsp; &nbsp;
[![Maintainability](https://api.codeclimate.com/v1/badges/3982e4436bbd2779606f/maintainability)](https://codeclimate.com/github/Ontica/Empiria.FinancialAccounting/maintainability)

Este producto de software está siendo desarrollado a la medida para el Banco Nacional de Obras y Servicios Públicos, S.N.C (BANOBRAS).

[BANOBRAS](https://www.gob.mx/banobras) es una institución de banca de desarrollo mexicana cuya labor
es financiar obras para la creación de servicios públicos. Por el tamaño de su cartera de crédito directo,
es el cuarto Banco más grande del sistema bancario mexicano y el primero de la Banca de Desarrollo de nuestro país.

Este repositorio contiene los módulos del *backend* del **Sistema de contabilidad financiera**.

Todos los módulos están escritos en C# 7.0 y utilizan .NET Framework versión 4.8.  
Los módulos pueden ser compilados utilizando [Visual Studio 2022 Community Edition](https://visualstudio.microsoft.com/vs/community/).

El acceso a los servicios que ofrece el *backend* se realiza mediante llamadas a servicios web de tipo RESTful,
mismos que están basados en ASP .NET.

Al igual que otros productos Empiria, este *backend* se apoya en [Empiria Framework](https://github.com/Ontica/Empiria.Core),
y también en algunos módulos de [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

La versión anterior de este sistema, la cual operó en BANOBRAS durante más de 20 años, 
también fue desarrollada por nuestra organización en el período 2000-2002,
y el código fuente de la misma se puede consultar [aquí](https://github.com/Ontica/Sicofin-2002).


## Contenido

El *backend* del **Sistema de contabilidad financiera** se conforma de los siguientes módulos:

1.  **Core**  
    Tipos, clases y servicios de propósito general que conforman el núcleo del *backend*.  

2.  **Balance Engine**  
    Provee servicios para el cálculo de saldos y balanzas de comprobación.  

3.  **External Data**  
    Administra las variables externas al sistema de contabilidad, así como sus valores.  

4.  **Financial Concepts**  
    Proporciona servicios para definir conceptos financieros y sus reglas de integración.  

5.  **Financial Reports**  
    Genera información para reportes financieros apoyándose de los servicios de *Balance Engine* y *Financial Concepts*.  

6.  **Fixed Assets Depreciation**  
    Proporciona información y servicios para el manejo de la depreciación del activo fijo.  

7.  **Reconciliation**  
    Provee servicios de conciliación de saldos y movimientos contables contra operaciones originadas en otros sistemas.  

8.  **Reporting**  
    Genera reportes fiscales, reportes de contabilidad financiera y reportes operativos.  

9.  **Vouchers Management**  
    Provee servicios para la edición, importación y administración de las pólizas contables.  

10. **Web API**  
    Capa de servicios web HTTP/Json para interactuar con todos los módulos que conforman el *backend* del Sistema.  


## Licencia

Este producto y sus partes se distribuyen mediante una licencia GNU AFFERO
GENERAL PUBLIC LICENSE, para uso exclusivo de BANOBRAS y de su personal, y
también para su uso por cualquier otro organismo en México perteneciente a
la Administración Pública Federal.

Para cualquier otro uso (con excepción a lo estipulado en los Términos de
Servicio de GitHub), es indispensable obtener con nuestra organización una
licencia distinta a esta.

Lo anterior restringe la distribución, copia, modificación, almacenamiento,
instalación, compilación o cualquier otro uso del producto o de sus partes,
a terceros, empresas privadas o a su personal, sean o no proveedores de
servicios de las entidades públicas mencionadas.

El desarrollo, evolución y mantenimiento de este producto está siendo pagado
en su totalidad con recursos públicos, y está protegido por las leyes nacionales
e internacionales de derechos de autor.

## Copyright

Copyright © 2021-2024. La Vía Óntica SC, Ontica LLC y autores.
Todos los derechos reservados.
