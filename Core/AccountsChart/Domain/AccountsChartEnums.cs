/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Enumerations                            *
*  Type     : AccountRole & DebtorCreditorType           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains enumerations used by the accounts and the accounts chart.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

namespace Empiria.FinancialAccounting {

  /// <summary>Describes the posting or summary role of an account.</summary>
  public enum AccountRole {

    Undefined = 'U',

    /// <summary>Summary account (cuenta sumaria).</summary>
    Sumaria = 'S',

    /// <summary>Posting account (cuenta de detalle).</summary>
    Detalle = 'P',

    /// <summary>Control account (cuenta de control que se maneja a nivel auxiliar).</summary>
    Control = 'C',

    /// <summary>Sectorized account (cuenta que maneja sector, con o sin auxiliares).</summary>
    Sectorizada = 'X',

  }  // enum AccountRole



  /// <summary>Enumerates an account debtor/creditor type (naturaleza deudora o acreedora).</summary>
  public enum DebtorCreditorType {

    Undefined = 'U',

    /// <summary>Debtor account (naturaleza deudora).</summary>
    Deudora = 'D',

    /// <summary>Creditor account (naturaleza acreedora).</summary>
    Acreedora = 'A'

  }  // enum DebtorCreditorType

}  // namespace Empiria.FinancialAccounting
