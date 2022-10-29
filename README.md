# Empiria Financial Accounting

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3eb36cbce7564607855c8995a3796d77)](https://www.codacy.com/gh/Ontica/Empiria.FinancialAccounting/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Ontica/Empiria.FinancialAccounting&amp;utm_campaign=Badge_Grade)
&nbsp; &nbsp;
[![Maintainability](https://api.codeclimate.com/v1/badges/3982e4436bbd2779606f/maintainability)](https://codeclimate.com/github/Ontica/Empiria.FinancialAccounting/maintainability)

This repository contains the Empiria Financial Accounting System's backend modules,
in C# 7.0 and .NET Framework 4.8, with a Http/Json Web Api integration module
tailored with ASP .NET.

As other Empiria products, this backend runs over [Empiria Framework](https://github.com/Ontica/Empiria.Core),
and as usual, needs some of the [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

The main design of the backend was took from SICOFIN, a custom-tailored Financial Accounting Web System,
developed by our organization in 2000-2002, for Banco Nacional de Obras y Servicios Públicos S.N.C.
(BANOBRAS), a Mexican state owned development bank. The original SICOFIN's source code
[can be downloaded here](https://github.com/Ontica/Sicofin).

## Contents

Empiria Financial Accounting comprises the following modules:

1.  **Core**
    Financial Accounting core domain classes and types.

2.  **Core Tests**
    Test suite for Financial Accounting core domain classes, types and services.

3.  **Balance Engine**
    Provides account's balances and trial balances services.

4.  **Balance Engine Tests**
    Tests suite for the Balance Engine services.

5.  **External Data**
    Provides read and write services for financial external variables and their values.

6.  **Financial Concepts**
	Provides services that manage financial concepts and their integrations.

7.  **Financial Reports**
    Generates information for financial reports based on accounts balance data structures and financial concepts and rules.

8.  **Reconciliation**
    Provides services for account's balance reconciliation.

9.  **Reporting**
    Generate financial accounting reports and export them to files.

10. **Vouchers Management**
    Provides services for vouchers edition, importation and management.

11. **Vouchers Management Tests**
    Tests suite for vouchers edition, importation and management.

12. **Web API**
    Http/Json RESTful interface for Financial Accounting system.

13. **Banobras Integration**
    Provides services used to integrate SICOFIN with other BANOBRAS systems.


Each single project can be compiled using Visual Studio 2019 Community Edition.

## Licencia

Este producto y sus partes se distribuyen mediante una licencia GNU AFFERO
GENERAL PUBLIC LICENSE, para uso exclusivo de BANOBRAS y de su personal, y
para su uso por cualquier otro organismo en México perteneciente a la
Administración Pública Federal.

Para cualquier otro uso (con excepción a lo estipulado en los Términos de
Servicio de GitHub), es indispensable obtener con nuestra organización una
licencia distinta a esta.

Lo anterior restringe la distribución, copia, modificación, almacenamiento,
instalación, compilación o cualquier otro uso del producto o de sus partes,
a terceros, empresas privadas o a su personal, sean o no proveedores de
servicios de las entidades públicas mencionadas.

El desarrollo de este producto fue pagado en su totalidad con recursos
públicos, y está protegido por las leyes nacionales e internacionales
de derechos de autor.

## Copyright

Copyright © 2021-2022. La Vía Óntica SC, Ontica LLC y autores.
Todos los derechos reservados.
