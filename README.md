# Empiria Financial Accounting

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/3eb36cbce7564607855c8995a3796d77)](https://www.codacy.com/gh/Ontica/Empiria.FinancialAccounting/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Ontica/Empiria.FinancialAccounting&amp;utm_campaign=Badge_Grade)
&nbsp; &nbsp;
[![Maintainability](https://api.codeclimate.com/v1/badges/3982e4436bbd2779606f/maintainability)](https://codeclimate.com/github/Ontica/Empiria.FinancialAccounting/maintainability)

This repository contains the Empiria Financial Accounting System's backend modules,
in C# 8.0 and .NET Framework 4.8, with a Http/Json Web Api integration module
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

5.  **Financial Reports**  
    Generates information for financial reports based on accounts balance data structures and financial concepts and rules.

6.  **Reconciliation**  
    Provides services for account's balance reconciliation.

7.  **Reporting**  
    Generate financial accounting reports and export them to files.

8.  **Rules**  
    Manages the repository of financial concepts and their integration rules.

9.  **Banobras Integration**  
    Import and export data services according to Banobras data models and data infrastructure.

10.  **Vouchers Management**  
    Provides services for vouchers edition, importation and management.

11. **Vouchers Management Tests**  
    Tests suite for vouchers edition, importation and management.

12. **Web API**  
    Http/Json RESTful interface for Financial Accounting system.

Each single project can be compiled using Visual Studio 2019 Community Edition.

## License

This system is distributed by the GNU AFFERO GENERAL PUBLIC LICENSE.

Óntica always delivers open source information systems, and we consider that it is specially
important in the case of public utility or government systems.

## Copyright

Copyright © 2000-2022. La Vía Óntica SC, Ontica LLC and colaborators.
