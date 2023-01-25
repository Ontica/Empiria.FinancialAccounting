/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Interface adapters                      *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Mapper class                            *
*  Type     : SaldosEncerradosMapper                     License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Methods used to map saldos encerrados.                                                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Empiria.FinancialAccounting.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine.Adapters {
  
  /// <summary></summary>
  static internal class SaldosEncerradosMapper {


    #region Public methods


    static public SaldosEncerradosDto Map(FixedList<SaldosEncerradosBaseEntryDto> mappedEntries) {
      return new SaldosEncerradosDto {
        Columns = DataColumns(),
        Entries = mappedEntries
      };
    }


    static public FixedList<SaldosEncerradosEntryDto> MergeBalancesIntoLockedUpBalanceEntries(
                   List<BalanzaTradicionalEntryDto> entries, FixedList<AccountDescriptorDto> accounts) {

      var mapped = entries.Select(x => MapToLockedUpEntry(x, accounts));

      return new FixedList<SaldosEncerradosEntryDto>(mapped);
    }


    #endregion Public methods


    #region Private methods


    static private void AccountClauses(SaldosEncerradosEntryDto dto,
                                       BalanzaTradicionalEntryDto entry,
                                       AccountDescriptorDto account) {
      if (entry.SubledgerAccountNumber.Length > 1) {

        dto.AccountNumber = entry.AccountNumberForBalances;
        dto.SubledgerAccount = entry.SubledgerAccountNumber;

      } else {
        dto.AccountNumber = entry.AccountNumber;
        dto.SubledgerAccount = "";

      }

      if (account.Role != AccountRole.Control) {
        dto.ItemType = TrialBalanceItemType.Summary;
        dto.IsCancelable = true;

      } else {

        dto.ItemType = entry.ItemType;
        if (entry.ItemType == TrialBalanceItemType.Entry) {
          dto.IsCancelable = true;
        }

      }
    }


    static private FixedList<DataTableColumn> DataColumns() {
      var columns = new List<DataTableColumn>();

      columns.Add(new DataTableColumn("currencyCode", "Mon", "text"));
      columns.Add(new DataTableColumn("accountNumber", "Cuenta", "text"));
      columns.Add(new DataTableColumn("itemName", "Nombre", "text"));
      columns.Add(new DataTableColumn("sectorCode", "Sector", "text"));
      columns.Add(new DataTableColumn("subledgerAccount", "Auxiliar", "text"));
      columns.Add(new DataTableColumn("lockedBalance", "Saldo encerrado", "decimal"));
      columns.Add(new DataTableColumn("roleChangeDate", "Fecha cambio Rol", "date"));
      columns.Add(new DataTableColumn("roleChange", "Rol", "text-button"));

      return columns.ToFixedList();
    }


    static private SaldosEncerradosEntryDto MapToLockedUpEntry(
                   BalanzaTradicionalEntryDto entry, FixedList<AccountDescriptorDto> accounts) {

      var account = accounts.Find(a => a.Number == entry.AccountNumberForBalances);

      var dto = new SaldosEncerradosEntryDto();
      AccountClauses(dto, entry, account);
      dto.StandardAccountId = entry.StandardAccountId;
      dto.CurrencyCode = entry.CurrencyCode;
      dto.LedgerUID = entry.LedgerUID;
      dto.LedgerNumber = entry.LedgerNumber;
      dto.LedgerName = entry.LedgerName;
      dto.RoleChangeDate = account.EndDate;
      dto.RoleChange = $"{account.Role}-{entry.AccountRole}";
      dto.ItemName = entry.AccountName;
      dto.SectorCode = entry.SectorCode;
      dto.LockedBalance = (decimal) entry.CurrentBalance;
      dto.LastChangeDate = entry.LastChangeDate;
      dto.NewRole = entry.AccountRole.ToString();
      dto.DebtorCreditor = entry.DebtorCreditor;

      return dto;
    }


    #endregion Private methods


  } // class SaldosEncerradosMapper

} // namespace Empiria.FinancialAccounting.BalanceEngine.Adapters
