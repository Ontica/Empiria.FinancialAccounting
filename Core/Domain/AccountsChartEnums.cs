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

    /// <summary>Summary account (cuenta sumaria).</summary>
    Summary = 'S',

    /// <summary>Posting account (cuenta de detalle).</summary>
    Posting = 'P',

    /// <summary>Control account (cuenta de control que se maneja a nivel auxiliar).</summary>
    Control = 'C',

    /// <summary>Sectorized account (cuenta que maneja sector, con o sin auxiliares).</summary>
    Sectorized = 'X',

  }  // enum AccountRole



  /// <summary>Enumerates an account debtor/creditor type (naturaleza deudora o acreedora).</summary>
  public enum DebtorCreditorType {

    /// <summary>Debtor account (naturaleza deudora).</summary>
    Debtor = 'D',

    /// <summary>Creditor account (naturaleza acreedora).</summary>
    Creditor = 'A'

  }  // enum DebtorCreditorType


}  // namespace Empiria.FinancialAccounting
