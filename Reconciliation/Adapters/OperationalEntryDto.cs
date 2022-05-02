/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reconciliation Services                    Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reconciliation.dll     Pattern   : Data Transfer Object                    *
*  Type     : OperationalEntryDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : DTO that describes an operational entry used for reconciliation processes.                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.FinancialAccounting.Reconciliation.Adapters {

  /// <summary>DTO that describes an operational entry used for reconciliation processes.</summary>
  internal class OperationalEntryDto {

    internal OperationalEntryDto() {
      // no-op
    }

    public string UniqueKey {
      get; internal set;
    } = string.Empty;


    public string LedgerNumber {
      get; internal set;
    } = Ledger.Empty.Number;


    public string AccountNumber {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountNumber {
      get; internal set;
    } = string.Empty;


    public string CurrencyCode {
      get; internal set;
    } = Currency.Empty.Code;


    public string SectorCode {
      get; internal set;
    } = Sector.Empty.Code;


    public string TransactionSlip {
      get; internal set;
    } = string.Empty;


    public JsonObject ExtData {
      get; internal set;
    } = new JsonObject();


    public decimal InitialBalance {
      get; internal set;
    }

    public decimal Debits {
      get; internal set;
    }

    public decimal Credits {
      get; internal set;
    }

    public decimal EndBalance {
      get; internal set;
    }

    public int Position {
      get; internal set;
    }

  }  // class OperationalEntryDto

}  // namespace Empiria.FinancialAccounting.Reconciliation.Adapters
