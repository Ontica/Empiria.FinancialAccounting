/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : LedgerDto                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO with data related to an accounting ledger book.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO with data related to an accounting ledger book.</summary>
  public class LedgerDto {

    internal LedgerDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }

    public string FullName {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }


    public string Subnumber {
      get; internal set;
    }


    public string SubsidiaryAccountsPrefix {
      get;
      internal set;
    }


    public NamedEntityDto AccountsChart {
      get; internal set;
    }


    public NamedEntityDto BaseCurrency {
      get; internal set;
    }

  }  // public class LedgerDto


  /// <summary>Output DTO for a ledger account.</summary>
  public class LedgerAccountDto {

    internal LedgerAccountDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }


    public NamedEntityDto Ledger {
      get; internal set;
    }


    public string Number {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public string AccountType {
      get; internal set;
    }


    public AccountRole Role {
      get; internal set;
    }


    public DebtorCreditorType DebtorCreditor {
      get; internal set;
    }


    public int Level {
      get; internal set;
    }


    public FixedList<NamedEntityDto> Currencies {
      get; internal set;
    }


    public FixedList<NamedEntityDto> Sectors {
      get; internal set;
    }

  }  // class LedgerAccountDto


  public class LedgerRuleDto {

    internal LedgerRuleDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public int Id {
      get; internal set;
    }

    public NamedEntityDto Ledger {
      get; internal set;
    }

    public DateTime StartDate {
      get; internal set;
    }

    public DateTime EndDate {
      get; internal set;
    }

  }

}  // namespace Empiria.FinancialAccounting.Adapters
