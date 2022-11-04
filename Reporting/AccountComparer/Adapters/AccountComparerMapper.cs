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

      columns.Add(new DataTableColumn("activeAccount", "Activas", "text-nowrap"));
      columns.Add(new DataTableColumn("activeBalance", "Saldo Activas", "decimal"));
      columns.Add(new DataTableColumn("pasiveAccount", "Pasivas", "text-nowrap"));
      columns.Add(new DataTableColumn("pasiveBalance", "Saldo Pasivo", "decimal"));
      columns.Add(new DataTableColumn("balanceDifference", "Diferencia", "decimal"));

      return columns.ToFixedList();
    }


    static private FixedList<IReportEntryDto> MapToReportDataEntries(List<AccountComparerEntry> entries) {

      var mappedItems = entries.Select((x) => MapToAccountComparerDto((AccountComparerEntry) x));

      return new FixedList<IReportEntryDto>(mappedItems);
    }


    static private AccountComparerEntryDto MapToAccountComparerDto(AccountComparerEntry x) {
      var dto = new AccountComparerEntryDto();

      dto.AccountGroupId = x.AccountGroupId;
      dto.ActiveAccount = x.ActiveAccount;
      dto.ActiveBalance = x.ActiveBalance;
      dto.PasiveAccount = x.PasiveAccount;
      dto.PasiveBalance = x.PasiveBalance;
      dto.BalanceDifference = x.BalanceDifference;

      return dto;
    }

    #endregion Private methods

  } // class AccountComparerMapper

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters
