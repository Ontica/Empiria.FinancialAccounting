/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Enumerated type                         *
*  Type     : AccountEditionCommandType                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Enumerated constants type used to classify AccountEditionCommand types.                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Enumerated constants type used to classify AccountEditionCommand types.</summary>
  public enum AccountEditionCommandType {

    AddCurrencies,

    AddSectors,

    CreateAccount,

    RemoveAccount,

    RemoveCurrencies,

    RemoveSectors,

    UpdateAccount,

    Undefined

  }  // enum AccountEditionCommandType

}  // namespace Empiria.FinancialAccounting.Adapters
