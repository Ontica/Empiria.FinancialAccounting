/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Data Transfer Object                    *
*  Type     : ResumenAjusteAnualDto                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO used to return a Balanza resumen ajuste anual diaria por moneda.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {

  /// <summary>Output DTO used to return a Balanza resumen ajuste anual diaria por moneda</summary>
  public class ResumenAjusteAnualDto {

    public TrialBalanceQuery Query {
      get; internal set;
    }


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    }


    public FixedList<ResumenAjusteAnualEntryDto> Entries {
      get; internal set;
    }

  } // class ResumenAjusteAnualDto


  /// <summary>Output DTO used to return a Balanza resumen ajuste anual diaria por moneda entries</summary>
  public class ResumenAjusteAnualEntryDto : ITrialBalanceEntryDto {


    public string MonthsInYear {
      get; internal set;
    }


    public decimal Debit {
      get; internal set;
    }


    public decimal Credit {
      get; internal set;
    }


    public decimal AverageBalanceCredit {
      get; internal set;
    }


    public decimal AverageBalanceDebit {
      get; internal set;
    }


    /// <summary>Credit > Debit</summary>
    public decimal DeductibleAdjustment {
      get; internal set;
    }


    /// <summary>Debit > Credit</summary>
    public decimal CumulativeAdjustment {
      get; internal set;
    }


    /// <summary>Cumulative/Deductible</summary>
    public decimal InflationAdjustmentMonths {
      get; internal set;
    }


    /// <summary>Cumulative/Deductible</summary>
    public decimal InflationAdjustment12Months {
      get; internal set;
    }


    public decimal INPCLastMonthPreviousYear {
      get; internal set;
    }


    public decimal INPCPreviousYear {
      get; internal set;
    }


    public decimal INPC {
      get; internal set;
    }


    public decimal FactorMonthsElapsed {
      get; internal set;
    }


    public decimal Factor12Months {
      get; internal set;
    }


    public TrialBalanceItemType ItemType => new TrialBalanceItemType();

    public string AccountNumber => string.Empty;

    public string SectorCode => string.Empty;

    public string SubledgerAccountNumber => string.Empty;

  } // class ResumenAjusteAnualEntryDto

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
