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
using Empiria.FinancialAccounting.Reporting.AccountComparer.Domain;

namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters {

  /// <summary>Methods used to map account comparer.</summary>
  static internal class AccountComparerMapper {

    #region Public methods

    static internal AccountComparerDto Map(FixedList<AccountComparerEntry> accounts, ReportBuilderQuery query) {
      return new AccountComparerDto {
        Query = query,
        Columns = GetColumns(),
        Entries = Map(accounts)
      };
    }



    #endregion Public methods


    #region Private methods


    static private FixedList<DataTableColumn> GetColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("accountName", "Nombre", "text"));

      return columns.ToFixedList();
    }


    static private FixedList<AccountComparerEntryDto> Map(FixedList<AccountComparerEntry> accounts) {
      var mappedItems = accounts.Select((x) => MapToAccountComparer((AccountComparerEntry) x));
      
      return new FixedList<AccountComparerEntryDto>(mappedItems);
    }


    static private AccountComparerEntryDto MapToAccountComparer(AccountComparerEntry x) {
      var comparerDto = new AccountComparerEntryDto();

      return comparerDto;
    }


    #endregion Private methods

  } // class AccountComparerMapper

} // namespace Empiria.FinancialAccounting.Reporting.AccountComparer.Adapters
