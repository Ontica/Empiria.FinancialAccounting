/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountsChartEditionFileReader             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Reads account edition commands contained in an Excel file.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Storage;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Reads account edition commands contained in an Excel file.</summary>
  internal class AccountsChartEditionFileReader {

    private readonly AccountsChart _chart;
    private readonly DateTime _applicationDate;
    private readonly InputFile _excelFile;
    private readonly bool _dryRun;

    public AccountsChartEditionFileReader(AccountsChart chart,
                                          DateTime applicationDate,
                                          InputFile excelFile,
                                          bool dryRun) {
      _chart = chart;
      _applicationDate = applicationDate;
      _excelFile = excelFile;
      _dryRun = dryRun;
    }


    internal FixedList<AccountEditionCommand> GetCommands() {
      return new FixedList<AccountEditionCommand>();
    }


  }  // class AccountsChartEditionFileReader

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
