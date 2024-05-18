/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Mapper class                            *
*  Type     : AccountComparerMapper                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map account comparer.                                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.DynamicData;

using Empiria.FinancialAccounting.Reporting.AccountComparer.Domain;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters {

  /// <summary>Methods used to map account comparer.</summary>
  static internal class AccountComparerMapper {

    #region Public methods

    static internal ReportDataDto MapToReportDataDto(ReportBuilderQuery buildQuery,
                                             List<AccountComparerEntry> entries) {

      return new ReportDataDto {
        Query = buildQuery,
        Columns = GetColumns(),
        Entries = MapToReportDataEntries(entries)
      };

    }



    #endregion Public methods


    #region Private methods


    static private FixedList<DataTableColumn> GetColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("currencyCode", "Moneda", "text-nowrap"));
      columns.Add(new DataTableColumn("activeAccount", "Cuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("activeAccountName", "Nombre", "text-nowrap"));
      columns.Add(new DataTableColumn("activeBalance", "Saldo", "decimal"));
      columns.Add(new DataTableColumn("pasiveAccount", "Contracuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("pasiveAccountName", "Nombre Contracuenta", "text-nowrap"));
      columns.Add(new DataTableColumn("pasiveBalance", "Saldo", "decimal"));
      columns.Add(new DataTableColumn("balanceDifference", "Diferencia", "decimal"));

      return columns.ToFixedList();
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(List<AccountComparerEntry> entries) {

      var mappedItems = entries.Select((x) => MapToAccountComparerDto((AccountComparerEntry) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private AccountComparerEntryDto MapToAccountComparerDto(AccountComparerEntry x) {
      var dto = new AccountComparerEntryDto();

      dto.ItemType = x.ItemType;
      dto.AccountGroupId = x.AccountGroupId;
      dto.CurrencyCode = x.CurrencyCode;
      dto.ActiveAccountName = x.AccountName;
      dto.ActiveAccount = x.AccountNumber;
      dto.ActiveBalance = x.AccountBalance;
      dto.PasiveAccountName = x.TargetAccountName;
      dto.PasiveAccount = x.TargetAccountNumber;
      dto.PasiveBalance = x.TargetBalance;
      dto.BalanceDifference = x.BalanceDifference;

      return dto;
    }

    #endregion Private methods

  } // class AccountComparerMapper

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters
