using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;
using Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Domain;

namespace Empiria.FinancialAccounting.Reporting.ValorizacionEstimacionPreventiva.Adaptars {
  
  static internal class ValorizacionPreventivaMapper {


    #region Public methods

    static internal ReportDataDto Map(ReportBuilderQuery query, TrialBalanceDto trialBalance) {
      
      var entries = trialBalance.Entries.Select(a => (ValorizacionEntryDto) a);

      return new ReportDataDto {
        Query = query,
        Columns = trialBalance.Columns,
        Entries = MapToReportDataEntries(entries.ToFixedList())
      };
    }


    private static FixedList<IReportEntryDto> MapToReportDataEntries(FixedList<ValorizacionEntryDto> entries) {

      var mappedItems = entries.Select((x) => MapEntry(x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    public static ValorizacionPreventivaEntryDto MapEntry(ValorizacionEntryDto entry) {

      var dto = new ValorizacionPreventivaEntryDto();

      dto.ItemType = entry.ItemType;
      dto.StandardAccountId = entry.StandardAccountId;
      dto.AccountName = entry.AccountName;
      dto.AccountNumber = entry.AccountNumber;
      dto.CurrencyCode = entry.CurrencyCode;
      dto.ValuedExchangeRate = entry.ValuedExchangeRate;
      dto.LastChangeDate = entry.LastChangeDate;

      AssignValuesByCurrency(dto, entry);

      dynamic obj = dto;

      SetTotalsFields(obj, entry);

      return obj;
    }


    #endregion Public methods


    #region Private methods


    static private void AssignValuesByCurrency(ValorizacionPreventivaEntryDto dto, ValorizacionEntryDto entry) {

      dto.USD = entry.USD;
      dto.EUR = entry.EUR;
      dto.YEN = entry.YEN;
      dto.UDI = entry.UDI;

      dto.LastUSD = entry.LastUSD;
      dto.LastYEN = entry.LastYEN;
      dto.LastEUR = entry.LastEUR;
      dto.LastUDI = entry.LastUDI;

      dto.CurrentUSD = entry.CurrentUSD;
      dto.CurrentYEN = entry.CurrentYEN;
      dto.CurrentEUR = entry.CurrentEUR;
      dto.CurrentUDI = entry.CurrentUDI;

      dto.ValuedEffectUSD = entry.ValuedEffectUSD;
      dto.ValuedEffectYEN = entry.ValuedEffectYEN;
      dto.ValuedEffectEUR = entry.ValuedEffectEUR;
      dto.ValuedEffectUDI = entry.ValuedEffectUDI;

      dto.TotalValued = entry.TotalValued;
      dto.TotalAccumulated = entry.TotalAccumulated;
    }


    private static void CurrenciesColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("usd", "USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("yen", "YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("eur", "EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("udi", "UDI", "decimal"));
      }
    }


    private static void CurrentMonthColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("currentUSD", "Mes actual USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("currentYEN", "Mes actual YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("currentEUR", "Mes actual EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("currentUDI", "Mes actual UDI", "decimal"));
      }
    }


    static private ColumnsByCurrency GetColumnsByCurrency(IEnumerable<ValorizacionPreventivaEntry> valorizations) {

      var columns = new ColumnsByCurrency();

      columns.USDColumn = valorizations.Sum(a => a.ValuesByCurrency.USD) > 0 ? true : false;
      columns.YENColumn = valorizations.Sum(a => a.ValuesByCurrency.YEN) > 0 ? true : false;
      columns.EURColumn = valorizations.Sum(a => a.ValuesByCurrency.EUR) > 0 ? true : false;
      columns.UDIColumn = valorizations.Sum(a => a.ValuesByCurrency.UDI) > 0 ? true : false;

      return columns;
    }


    static private void GetDynamicColumns(List<DataTableColumn> columns,
                        IEnumerable<ValorizacionPreventivaEntry> valorizations) {

      var entry = valorizations.FirstOrDefault();

      if (entry != null) {

        List<string> members = new List<string>();

        members.AddRange(entry.GetDynamicMemberNames());

        foreach (var member in members) {
          columns.Add(new DataTableColumn(member.ToLower(), member, "decimal"));
        }

      }

    }


    private static void LastMonthColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("lastUSD", "Mes anterior USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("lastYEN", "Mes anterior YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("lastEUR", "Mes anterior EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("lastUDI", "Mes anterior UDI", "decimal"));
      }
    }


    private static void ValuedEffectColumns(List<DataTableColumn> columns,
                                          ColumnsByCurrency columnsByCurrency) {
      if (columnsByCurrency.USDColumn) {
        columns.Add(new DataTableColumn("valuedEffectUSD", "Efecto valorización USD", "decimal"));
      }
      if (columnsByCurrency.YENColumn) {
        columns.Add(new DataTableColumn("valuedEffectYEN", "Efecto valorización YEN", "decimal"));
      }
      if (columnsByCurrency.EURColumn) {
        columns.Add(new DataTableColumn("valuedEffectEUR", "Efecto valorización EUR", "decimal"));
      }
      if (columnsByCurrency.UDIColumn) {
        columns.Add(new DataTableColumn("valuedEffectUDI", "Efecto valorización UDI", "decimal"));
      }
    }


    #endregion Private methods


    #region Helpers

    static private void SetTotalsFields(DynamicValorizacionEntryDto o, ValorizacionEntryDto entry) {
      var totalsColumns = entry.GetDynamicMemberNames();

      foreach (string column in totalsColumns) {
        o.SetTotalField(column, entry.GetTotalField(column));
      }
    }

    #endregion Helpers


  }
}
