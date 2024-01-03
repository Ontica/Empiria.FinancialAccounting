# Empiria Financial Accounting System

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3eb36cbce7564607855c8995a3796d77)](https://www.codacy.com/gh/Ontica/Empiria.FinancialAccounting/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Ontica/Empiria.FinancialAccounting&amp;utm_campaign=Badge_Grade)
&nbsp; &nbsp;
[![Maintainability](https://api.codeclimate.com/v1/badges/3982e4436bbd2779606f/maintainability)](https://codeclimate.com/github/Ontica/Empiria.FinancialAccounting/maintainability)

This repository contains the Empiria Financial Accounting System's backend modules,
in C# 7.0 and .NET Framework 4.8, with a Http/Json Web Api integration module
tailored with ASP .NET.

As other Empiria products, this backend runs over [Empiria Framework](https://github.com/Ontica/Empiria.Core),
and as usual, needs some of the [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

SICOFIN, a custom-tailored Financial Accounting Web System, was originally developed by our organization in 2000-2002,
for Banco Nacional de Obras y Servicios Públicos S.N.C. (BANOBRAS), a Mexican state owned development bank.
The original SICOFIN's source code [can be downloaded here](https://github.com/Ontica/Sicofin).

## Contents

Empiria Financial Accounting comprises the following modules:

1.  **Core**  
    Financial Accounting core domain classes and types.

2.  **Balance Engine**  
    Provides account's balances and trial balances services.

3.  **External Data**  
    Provides read and write services for financial external variables and their values.

4.  **Financial Concepts**  
    Provides services that manage financial concepts and their integrations.

5.  **Financial Reports**  
    Generates information for financial reports based on accounts balance data structures and financial concepts and rules.

6.  **Fixed Assets Depreciation**  
    Generates information for fixed assets depreciation.

7.  **Reconciliation**  
    Provides services for account's balance reconciliation.

8.  **Reporting**  
    Generates financial accounting reports and export them to files.

9.  **Vouchers Management**  
    Provides services for vouchers edition, importation and management.

10. **Web API**  
    Http/Json RESTful interface for Financial Accounting system.

11. **Banobras Integration**  
    Provides services used to integrate SICOFIN with other BANOBRAS systems.


Each single project can be compiled using Visual Studio 2022 Community Edition.

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

Copyright © 2021-2024. La Vía Óntica SC, Ontica LLC y autores.
Todos los derechos reservados.
