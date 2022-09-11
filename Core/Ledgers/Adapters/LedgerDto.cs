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


    public string SubledgerAccountsPrefix {
      get;
      internal set;
    }


    public FixedList<NamedEntityDto> SubledgerAccountsTypes {
      get; internal set;
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


    public int StandardAccountId {
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


    public NamedEntityDto AccountType {
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


    public FixedList<ValuedCurrencyDto> Currencies {
      get; internal set;
    }


    public FixedList<SectorRuleShortDto> Sectors {
      get; internal set;
    }

  }  // class LedgerAccountDto


  public class SectorRuleDto {

    internal SectorRuleDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public SectorDto Sector {
      get; internal set;
    }

    public AccountRole SectorRole {
      get; internal set;
    }

    public DateTime StartDate {
      get; internal set;
    }

    public DateTime EndDate {
      get; internal set;
    }

  }  // class SectorRuleDto


  public class SectorRuleShortDto {

    internal SectorRuleShortDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public string Code {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public AccountRole Role {
      get; internal set;
    }

  }  // class SectorRuleDto


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

  }  // class LedgerRuleDto


  /// <summary>Output DTO for currencies with an exchange rate value.</summary>
  public class ValuedCurrencyDto {

    internal ValuedCurrencyDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public decimal ExchangeRate {
      get; internal set;
    }

  }  // class ValuedCurrencyDto


}  // namespace Empiria.FinancialAccounting.Adapters
