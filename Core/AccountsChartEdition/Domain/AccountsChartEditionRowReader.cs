/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.dll                    Pattern   : Service provider                        *
*  Type     : AccountsChartEditionRowReader              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Gets account edition commands from a spreadsheet row.                                          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;
using Empiria.Office;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Gets account edition commands from a spreadsheet row.</summary>
  sealed internal class AccountsChartEditionRowReader {

    private readonly Spreadsheet _spreadsheet;
    private readonly int _rowIndex;

    public AccountsChartEditionRowReader(Spreadsheet spreadsheet, int rowIndex) {
      Assertion.Require(spreadsheet, nameof(spreadsheet));
      Assertion.Require(rowIndex > 0, "rowIndex must be greater than zero.");

      _spreadsheet = spreadsheet;
      _rowIndex = rowIndex;
    }


    internal AccountFieldsDto GetAccountFields() {
      return new AccountFieldsDto {
        AccountNumber = GetAccountNumber(),
        Name = GetAccountName(),
        Description = string.Empty,
        Role = GetAccountRole(),
        DebtorCreditor = DebtorCreditorType.Undefined,
        AccountTypeUID = string.Empty
      };
    }


    internal string GetAccountNumber() {
      return ReadStringValueFromColumn("B");
    }


    internal string GetAccountName() {
      return ReadStringValueFromColumn("C");
    }


    internal AccountRole GetAccountRole() {
      string mainRole = ReadStringValueFromColumn("D").ToLower();

      bool isSubsidary = ReadBoolValueFromColumn("E");
      bool hasSectors = GetSectors().Length != 0;

      if (mainRole == "sumaria") {
        return AccountRole.Sumaria;

      } else if (mainRole == "registro" && hasSectors) {
        return AccountRole.Sectorizada;

      } else if (mainRole == "registro" && isSubsidary) {
        return AccountRole.Control;

      } else if (mainRole == "registro" && !isSubsidary) {
        return AccountRole.Detalle;
      }

      return AccountRole.Undefined;
    }


    internal AccountEditionCommandType GetCommandType() {
      var value = ReadStringValueFromColumn("A").ToLower();

      if (value.Length == 0) {
        return AccountEditionCommandType.Undefined;
      }

      if (value == "agregar cuenta") {
        return AccountEditionCommandType.CreateAccount;
      }

      if (value.StartsWith("cambiar ")) {
        return AccountEditionCommandType.UpdateAccount;
      }

      return AccountEditionCommandType.Undefined;
    }


    internal string[] GetCurrencies() {
      string[] columns = new[] { "F", "G", "H", "I", "J" };

      return ReadStringArrayFromColumns(columns);
    }


    internal string[] GetDataToBeUpdated() {
      var value = ReadStringValueFromColumn("A");

      AccountEditionCommandType commandType = GetCommandType();

      if (commandType == AccountEditionCommandType.CreateAccount) {
        return new string[0];
      }

      if (commandType == AccountEditionCommandType.UpdateAccount) {
        value = value.Replace("Cambiar ", string.Empty);

        return value.Split(',').Select(x => EmpiriaString.TrimAll(x))
                               .ToArray();
      }

      return new string[0];
    }


    internal string GetCommandText() {
      return ReadStringValueFromColumn("A");
    }


    internal string[] GetSectors() {
      string[] columns = new[] { "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
                                 "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD" };

      return ReadStringArrayFromColumns(columns);
    }

    #region Helpers

    private bool ReadBoolValueFromColumn(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_rowIndex}").Trim();

      if (value.Length == 0 && value != "-") {
        return false;
      }

      return EmpiriaString.ToBoolean(EmpiriaString.TrimAll(value));
    }


    private string[] ReadStringArrayFromColumns(string[] columns) {
      var list = new List<string>();

      foreach (var column in columns) {
        var value = ReadStringValueFromColumn(column).Trim();

        if (value.Length != 0 && value != "-") {
          list.Add(value);
        }
      }

      return list.ToArray();
    }


    private string ReadStringValueFromColumn(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_rowIndex}");

      return EmpiriaString.TrimAll(value);
    }

    #endregion Helpers

  }  // class AccountsChartEditionRowReader

} // namespace Empiria.FinancialAccounting.AccountsChartEdition
