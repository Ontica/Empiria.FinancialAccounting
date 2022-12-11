/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Enumeration                             *
*  Type     : RoundTo                                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes a quantity round to operation.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Describes a quantity round to operation.</summary>
  public enum RoundTo {

    DoNotRound,

    Units,

    Thousands,

    Millions

  }  // RoundTo

}  // namespace Empiria.FinancialAccounting
