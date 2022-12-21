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
        Role = GetAccountRole()
      };
    }


    internal string GetAccountNumber() {
      var value = ReadStringValueFromColumn("B");

      Assertion.Require(value,
                  $"No se proporcionó el número de cuenta en la celda B{_rowIndex}.");

      return value;
    }


    internal string GetAccountName() {
      var value = ReadStringValueFromColumn("C");

      Assertion.Require(value,
                $"No se proporcionó la descripción o nombre de la " +
                $"cuenta en la celda C{_rowIndex}.");

      Assertion.Require(value.Length >= 4,
            $"La longitud de la descripción o nombre de la cuenta " +
            $"en la celda C{_rowIndex} es demasiado corta: '{value}'");

      return value;
    }


    internal AccountRole GetAccountRole() {
      string mainRole = ReadStringValueFromColumn("D").ToLower();

      Assertion.Require(mainRole,
                        $"No se proporcionó el rol de la cuenta en la celda D{_rowIndex}.");

      bool isSubsidiary = GetIsSubsidiaryFlag();
      bool hasSectors = GetSectors().Length != 0;

      if (mainRole == "sumaria") {
        return AccountRole.Sumaria;
      }

      Assertion.Require(mainRole == "registro" || mainRole == "detalle",
                        $"La celda D{_rowIndex} tiene un rol" +
                        $"que no reconozco: {ReadStringValueFromColumn("D")}");

      if (hasSectors) {
        return AccountRole.Sectorizada;

      } else if (isSubsidiary) {
        return AccountRole.Control;

      } else {
        return AccountRole.Detalle;
      }
    }


    internal bool GetIsSubsidiaryFlag() {
      bool? isSubsidiary = TryReadBoolValueFromColumn("E");

      Assertion.Require(isSubsidiary.HasValue,
                  $"No puedo determinar si la cuenta maneja auxiliares o no " +
                  $"con el dato de la celda E{_rowIndex}: {ReadStringValueFromColumn("E")}");

      return isSubsidiary.Value;
    }


    internal AccountRole GetSectorsRole() {
      bool isSubsidiary = GetIsSubsidiaryFlag();
      bool hasSectors = GetSectors().Length != 0;

      if (!hasSectors) {
        return AccountRole.Undefined;
      }

      return isSubsidiary ? AccountRole.Control : AccountRole.Detalle;
    }


    internal AccountEditionCommandType GetCommandType() {
      var value = ReadStringValueFromColumn("A").ToLower();

      value = EmpiriaString.RemoveAccents(value);

      if (value.Length == 0) {
        return AccountEditionCommandType.Undefined;
      }

      if (value == "agregar cuenta") {
        return AccountEditionCommandType.CreateAccount;
      }

      if (value == "corregir descripcion") {
        return AccountEditionCommandType.FixAccountName;
      }

      if (value.StartsWith("cambiar ") ||
          value.StartsWith("actualizar ") ||
          value.StartsWith("modificar ")) {
        return AccountEditionCommandType.UpdateAccount;
      }

      Assertion.RequireFail(
            $"No puedo determinar qué operación se desea realizar, " +
            $"de acuerdo a los datos de la celda A{_rowIndex}: {GetCommandText()}");

      return AccountEditionCommandType.Undefined;
    }


    internal string[] GetCurrencies() {
      string[] columns = new[] { "F", "G", "H", "I", "J" };

      return ReadStringArrayFromColumns(columns);
    }


    internal AccountDataToBeUpdated[] GetDataToBeUpdated() {
      var value = ReadStringValueFromColumn("A").ToLower();

      AccountEditionCommandType commandType = GetCommandType();

      if (commandType == AccountEditionCommandType.CreateAccount) {
        return new AccountDataToBeUpdated[0];
      }

      if (commandType == AccountEditionCommandType.FixAccountName) {
        return new AccountDataToBeUpdated[0];
      }

      if (commandType == AccountEditionCommandType.UpdateAccount) {
        value = value.Replace("cambiar ", string.Empty);
        value = value.Replace("actualizar ", string.Empty);
        value = value.Replace("modificar ", string.Empty);
        value = value.Replace(".", string.Empty);
        value = value.Replace(";", ",");

        var stringArray = value.Split(',').Select(x => EmpiriaString.TrimAll(x).ToLower())
                                    .ToArray();

        return CreateUpdatedAccountDataFromStringArray(stringArray);
      }

      return new AccountDataToBeUpdated[0];
    }


    private AccountDataToBeUpdated[] CreateUpdatedAccountDataFromStringArray(string[] stringArray) {
      var list = new List<AccountDataToBeUpdated>();

      foreach (var s in stringArray) {
        if (s == "descripción" || s == "descripcion" || s == "nombre") {
          list.Add(AccountDataToBeUpdated.Name);

        } else if (s == "rol") {
          list.Add(AccountDataToBeUpdated.MainRole);

        } else if (s == "auxiliar" || s == "auxiliares") {
          list.Add(AccountDataToBeUpdated.SubledgerRole);

        } else if (s == "moneda" || s == "monedas") {
          list.Add(AccountDataToBeUpdated.Currencies);

        } else if (s == "sector" || s == "sectores") {
          list.Add(AccountDataToBeUpdated.Sectors);

        } else {
          Assertion.RequireFail($"La fila {_rowIndex} del archivo contiene una " +
                                $"operación que no reconozco: {GetCommandText()}.");
        }
      }

      return list.ToArray();
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

    private bool? TryReadBoolValueFromColumn(string column) {
      var value = _spreadsheet.ReadCellValue<string>($"{column}{_rowIndex}").Trim();

      if (value.Length == 0 || value == "-") {
        return false;
      }

      return EmpiriaString.TryToBoolean(EmpiriaString.TrimAll(value));
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
