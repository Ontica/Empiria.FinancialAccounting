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

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters {

  /// <summary>Output DTO used to return 'Derrrama de intereses de swaps de cobertura' report data.</summary>
  public class DerramaSwapsCoberturaDto {


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
  public class DerramaSwapsCoberturaEntryDto : IReportEntryDto {

    public string ItemType {
      get; internal set;
    } = "Entry";


    public string SubledgerAccount {
      get; internal set;
    } = string.Empty;


    public string SubledgerAccountName {
      get; internal set;
    } = string.Empty;


    public string Classification {
      get; internal set;
    } = string.Empty;


    public decimal IncomeAccountTotal {
      get; internal set;
    }

    public decimal ExpensesAccountTotal {
      get; internal set;
    }

    public decimal Total {
      get {
        return IncomeAccountTotal + ExpensesAccountTotal;
      }
    }

  } // class DerramaSwapsCoberturaEntryDto


} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters
