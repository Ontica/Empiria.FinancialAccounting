/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                            Component : Report Builders                      *
*  Assembly : FinancialAccounting.Reporting.dll             Pattern   : Report builder                       *
*  Type     : DerramaSwapsCoberturaConsolidadoBuilder       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Generates report 'Derrama de intereses de Swaps con fines de cobertura consolidado'.           *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;

using Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura.Adapters;

namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura {

  /// <summary>Generates report 'Derrama de intereses de Swaps con fines de cobertura consolidado'.</summary>
  internal class DerramaSwapsCoberturaConsolidadoBuilder : IReportBuilder {

    #region Public methods

    public ReportDataDto Build(ReportBuilderQuery buildQuery) {
      Assertion.Require(buildQuery, nameof(buildQuery));

      FixedList<SwapsCoberturaListConfigItem> reportItems = SwapsCoberturaList.Parse()
                                                                              .GetConfiguration();

      FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> reportBaseData = GetReportBaseData(buildQuery);

      var reportTotals = MergeEntries(reportItems, reportBaseData);

      return DerramaSwapsCoberturaConsolidadoMapper.MapToReportDataDto(buildQuery, reportTotals);
    }

    #endregion Public methods

    #region Private methods


    private FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> GetReportBaseData(ReportBuilderQuery buildQuery) {
      var baseReport = new DerramaSwapsCoberturaBuilder();

      var baseData = baseReport.Build(buildQuery);

      return baseData.Entries.FindAll(x => ((DerramaSwapsCoberturaEntryDto) x).ItemType != "Total")
                             .Select(x => (DerramaSwapsCoberturaEntryDto) x)
                             .GroupBy(x => x.Classification)
                             .Select(x => new DerramaSwapsCoberturaConsolidadoEntryDto {
                               ItemType = x.First().ItemType,
                               Classification = x.First().Classification,
                               IncomeTotal = x.Sum(y => y.IncomeAccountTotal),
                               ExpensesTotal = x.Sum(y => y.ExpensesAccountTotal)
                             }).ToFixedList();
    }


    private FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> MergeEntries(FixedList<SwapsCoberturaListConfigItem> reportItems,
                                                                             FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> itemTotals) {
      var list = new List<DerramaSwapsCoberturaConsolidadoEntryDto>(reportItems.Count);

      foreach (var reportItem in reportItems) {
        if (reportItem.IsTotalRow && reportItem.Group != string.Empty) {
          list.Add(BuildSubtotalRow(reportItems, reportItem, itemTotals));
        } else if (reportItem.IsTotalRow && reportItem.Group == string.Empty) {
          list.Add(BuildTotalRow(reportItem, itemTotals));
        } else {
          list.Add(BuildClassificationRow(reportItem, itemTotals));
        }
      }

      RemoveUnclassificatedRow(list);

      return list.ToFixedList();
    }

    private void RemoveUnclassificatedRow(List<DerramaSwapsCoberturaConsolidadoEntryDto> list) {
      var item = list.Find(x => x.Classification == "Sin clasificar");

      if (item == null) {
        return;
      }

      if (item.IncomeTotal == 0 && item.ExpensesTotal == 0) {
        list.Remove(item);
      }
    }

    private DerramaSwapsCoberturaConsolidadoEntryDto BuildSubtotalRow(FixedList<SwapsCoberturaListConfigItem> configItems,
                                                                      SwapsCoberturaListConfigItem totalConfigItem,
                                                                      FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> itemTotals) {

      var item = new DerramaSwapsCoberturaConsolidadoEntryDto {
        ItemType = "Total",
        Row = totalConfigItem.Row
      };

      var totalGroupItems = configItems.FindAll(x => x.Group == totalConfigItem.Group);

      foreach (var configItem in totalGroupItems) {
        var itemTotal = itemTotals.Find(x => x.Classification == configItem.Value);

        if (itemTotal != null) {
          item.IncomeTotal += itemTotal.IncomeTotal;
          item.ExpensesTotal += itemTotal.ExpensesTotal;
        }
      }

      return item;
    }

    private DerramaSwapsCoberturaConsolidadoEntryDto BuildTotalRow(SwapsCoberturaListConfigItem totalConfigItem,
                                                                   FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> itemTotals) {

      var item = new DerramaSwapsCoberturaConsolidadoEntryDto {
        ItemType = "Total",
        Row = totalConfigItem.Row
      };

      foreach (var itemTotal in itemTotals) {
        item.IncomeTotal += itemTotal.IncomeTotal;
        item.ExpensesTotal += itemTotal.ExpensesTotal;
      }

      return item;
    }

    private DerramaSwapsCoberturaConsolidadoEntryDto BuildClassificationRow(SwapsCoberturaListConfigItem configItem,
                                                                            FixedList<DerramaSwapsCoberturaConsolidadoEntryDto> itemTotals) {
      var data = itemTotals.Find(x => x.Classification == configItem.Value);

      if (data != null) {
        data.Row = configItem.Row;
        return data;
      }

      return new DerramaSwapsCoberturaConsolidadoEntryDto {
        Classification = configItem.Value,
        Row = configItem.Row,
      };

    }


    #endregion Private methods

  } // class DerramaSwapsCoberturaConsolidadoBuilder

} // namespace Empiria.FinancialAccounting.Reporting.DerramaSwapsCobertura
