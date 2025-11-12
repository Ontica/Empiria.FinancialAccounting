/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : ResumenAjusteEntry                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for resumen de ajuste anual entry.                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Represents an entry for resumen de ajuste anual entry</summary>
  internal class ResumenAjusteAnualEntry : ITrialBalanceEntry {

    public DateTime FiscalYearDate {
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

  } // class ResumenAjusteAnualEntry

} // namespace Empiria.FinancialAccounting.BalanceEngine
