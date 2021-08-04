/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Command payload                         *
*  Type     : TrialBalanceCommandData                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Command payload used to build trial balances.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.BalanceEngine {

  internal class TrialBalanceCommandData {

    public AccountsChart AccountsChart {
      get; internal set;
    }

    public StoredBalanceSet StoredInitialBalanceSet {
      get; internal set;
    }

    public DateTime FromDate {
      get; internal set;
    }

    public DateTime ToDate {
      get; internal set;
    }

    public string Fields {
      get; internal set;
    } = string.Empty;


    public string InitialFields {
      get; set;
    } = string.Empty;


    public string Filters {
      get; internal set;
    } = string.Empty;


    public string AccountFilters {
      get; set;
    } = string.Empty;


    public string InitialGrouping {
      get; internal set;
    } = string.Empty;


    public string Grouping {
      get; internal set;
    } = string.Empty;


    public string Where {
      get; set;
    } = string.Empty;


    public string Ordering {
      get; internal set;
    } = string.Empty;


    public string LastChangeDateColumn {
      get; internal set;
    } = ", FECHA_AFECTACION ";

  } // class TrialBalanceCommandData

} // namespace Empiria.FinancialAccounting.BalanceEngine
