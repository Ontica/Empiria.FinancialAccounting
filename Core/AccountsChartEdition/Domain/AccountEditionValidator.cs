/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service provider                        *
*  Type     : AccountEditionValidator                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides account edition validation services.                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition {

  /// <summary>Provides account edition validation services.</summary>
  internal class AccountEditionValidator {

    #region Fields

    private readonly AccountsChart _accountsChart;
    private readonly AccountEditionCommand _command;
    private readonly List<string> _issuesList;

    #endregion Fields

    #region Constructors and parsers

    public AccountEditionValidator(AccountEditionCommand command) {
      Assertion.Require(command, nameof(command));

      _command = command;
      _accountsChart = command.GetAccountsChart();
      _issuesList = new List<string>(16);
    }

    #endregion Constructors and parsers

    #region Properties

    public FixedList<string> Issues {
      get {
        return _issuesList.ToFixedList();
      }
    }

    #endregion Properties

    #region Methods

    internal void AddIssue(string issue) {
      Assertion.Require(issue, nameof(issue));

      _issuesList.Add(issue);
    }


    internal bool EnsureCanAddCurrenciesTo(Account account) {
      FixedList<Currency> currenciesToBeAdded = _command.GetCurrencies();

      if (currenciesToBeAdded.Count == 0) {
        this.AddIssue("No se proporcionó la lista con las nuevas monedas a registrar.");

        return false;
      }

      bool isOK = true;

      foreach (Currency currencyToAdd in currenciesToBeAdded) {
        if (account.CurrencyRules.Contains(x => x.Currency.Equals(currencyToAdd))) {
          this.AddIssue($"La cuenta ya tiene registrada la moneda {currencyToAdd.FullName}.");
          isOK = false;

        } else if (!_accountsChart.MasterData.Currencies.Contains(currencyToAdd)) {
          this.AddIssue($"El catálogo de cuentas no maneja la moneda {currencyToAdd.FullName}.");
          isOK = false;

        }
      }

      return isOK;
    }


    internal bool EnsureCanAddSectorsTo(Account account) {
      FixedList<SectorInputRuleDto> sectorRulesToBeAdded = _command.GetSectorRules();

      if (sectorRulesToBeAdded.Count == 0) {
        this.AddIssue("No se proporcionó la lista con los nuevos sectores a registrar.");

        return false;
      }

      bool isOK = true;

      foreach (SectorInputRuleDto sectorRuleToAdd in sectorRulesToBeAdded) {
        if (account.SectorRules.Contains(x => x.Sector.Equals(sectorRuleToAdd))) {
          this.AddIssue($"La cuenta ya tiene registrado el sector {sectorRuleToAdd.Sector.FullName}.");
          isOK = false;

        } else if (sectorRuleToAdd.Sector.IsSummary) {
          this.AddIssue($"El sector {sectorRuleToAdd.Sector.FullName} tiene sectores hijos, " +
                        $"por lo que no puede ser asignado directamente a una cuenta.");
          isOK = false;

        } else if (!_accountsChart.MasterData.Sectors.Contains(sectorRuleToAdd.Sector)) {
          this.AddIssue($"El catálogo de cuentas no maneja el sector {sectorRuleToAdd.Sector.FullName}.");
          isOK = false;

        } else if (!(sectorRuleToAdd.Role == AccountRole.Detalle ||
                     sectorRuleToAdd.Role == AccountRole.Control)) {
          this.AddIssue($"El sector {sectorRuleToAdd.Sector.FullName} tiene un rol " +
                        $"que no es válido para sectores: {sectorRuleToAdd.Role}.");
          isOK = false;
        }
      }

      return isOK;
    }


    internal bool EnsureCanCreateAccount() {
      return false;
    }

    #endregion Methods

  }  // class AccountEditionValidator

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition
