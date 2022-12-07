using System;

using Empiria.FinancialAccounting.BalanceEngine.Adapters;

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
