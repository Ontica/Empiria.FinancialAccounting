/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Interface adapters                   *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Data Transfer Object                 *
*  Type     : DerramaSwapsCoberturaDto                      License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Output DTO used to return 'Derrrama de intereses de swaps de cobertura' report data.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.DynamicData;

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters {

  /// <summary>Output DTO used to return 'Derrrama de intereses de swaps de cobertura' report data.</summary>
  public class DerramaSwapsCoberturaConsolidadoDto {


    public ReportBuilderQuery Query {
      get; internal set;
    } = new ReportBuilderQuery();


    public FixedList<DataTableColumn> Columns {
      get; internal set;
    } = new FixedList<DataTableColumn>();


    public FixedList<DerramaSwapsCoberturaEntryDto> Entries {
      get; internal set;
    } = new FixedList<DerramaSwapsCoberturaEntryDto>();


  } // class DerramaSwapsCoberturaDto



  /// <summary>DTO for each account comparer entry report.</summary>
  public class DerramaSwapsCoberturaConsolidadoEntryDto : IReportEntryDto {

    public string ItemType {
      get; internal set;
    } = "Entry";

    public string Classification {
      get; internal set;
    } = string.Empty;


    public decimal IncomeTotal {
      get; internal set;
    }

    public decimal ExpensesTotal {
      get; internal set;
    }

    public decimal Total {
      get {
        return IncomeTotal + ExpensesTotal;
      }
    }

    public int Row {
      get; internal set;
    }

  } // class DerramaSwapsCoberturaEntryDto


} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters
