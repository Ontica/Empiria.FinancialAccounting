# Empiria Financial Accounting

This repository contains the Empiria Financial Accounting System's backend modules,
in C# 8.0 and .NET Framework 4.8, with a Http/Json Web Api integration module
tailored with ASP .NET.

As other Empiria products, this backend runs over [Empiria Framework](https://github.com/Ontica/Empiria.Core),
and as usual, needs some of the [Empiria Extensions](https://github.com/Ontica/Empiria.Extensions).

The main design of the backend was took from SICOFIN, a custom-tailored Financial Accounting Web System,
developed by our organization in 2000-2002, for Banco Nacional de Obras y Servicios Públicos S.N.C
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

3.  **Balance Engine Tests**  
    Tests suite for the Balance Engine services.

4.  **Web API**  
    Http/Json RESTful interface for Financial Accounting system.

Each single project can be compiled using Visual Studio 2019 Community Edition.

## License

This system is distributed by the GNU AFFERO GENERAL PUBLIC LICENSE.

Óntica always delivers open source information systems, and we consider that it is specially
important in the case of public utility or government systems.

## Copyright

Copyright © 2000-2021. La Vía Óntica SC, Ontica LLC and colaborators.
