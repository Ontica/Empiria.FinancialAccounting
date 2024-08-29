/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Service Provider                        *
*  Type     : AccountAssertions                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides business rules checking through assertions for financial accounts.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting {

  /// <summary>Provides business rules checking through assertions for financial accounts.</summary>
  public class AccountAssertions {

    private readonly Account _account;
    private readonly DateTime _accountingDate;

    public AccountAssertions(Account account, DateTime accountingDate) {
      Assertion.Require(account, nameof(account));

      _account = account;
      _accountingDate = accountingDate;
    }

    #region Assertions

    public void AssertCurrencyRule(Currency currency) {
      Assertion.Require(currency, nameof(currency));

      Assertion.Require(
          _account.GetCurrencies(_accountingDate).Contains(x => x.Currency.Equals(currency)),
          $"La cuenta {_account.Number} no permite movimientos en la moneda {currency.FullName}.");
    }


    public void AssertIsNotSummary() {
      Assertion.Require(_account.Role != AccountRole.Sumaria,
                        $"La cuenta {_account.Number} es sumaria, por lo que no admite movimientos.");
    }


    public void AssertIsNotProtectedForEdition() {
      Assertion.Require(!_account.Number.StartsWith("4.02.03.01") ||
                        ExecutionServer.CurrentPrincipal.IsInRole("administrador-operativo"),
                        $"La cuenta {_account.Number} está protegida contra edición.");

      //Assertion.Require(!_account.Number.StartsWith("9.01") ||
      //                  ExecutionServer.CurrentPrincipal.HasPermission("registro-manual-cuentas-protegidas"),
      //                  $"La cuenta {_account.Number} está protegida contra edición.");
    }


    public void AssertNoEventTypeRule() {
      if (_account.Number.StartsWith("13")) {
        Assertion.RequireFail($"La cuenta {_account.Number} necesita un tipo de evento, " +
                              $"sin embargo no se proporcionó.");
      }
    }


    public void AssertNoSectorRule() {
      Assertion.Require(_account.Role != AccountRole.Sectorizada,
                       $"La cuenta {_account.Number} requiere un sector, " +
                       $"sin embargo no se proporcionó.");
    }


    public void AssertSectorRule(Sector sector) {
      Assertion.Require(sector, nameof(sector));

      Assertion.Require(_account.Role == AccountRole.Sectorizada,
          $"La cuenta {_account.Number} no requiere sector, " +
          $"sin embargo se proporcionó el sector {sector.FullName}.");

      Assertion.Require(
          _account.GetSectors(_accountingDate).Contains(x => x.Sector.Equals(sector)),
          $"El sector {sector.Code} no está definido para la cuenta {_account.Number}.");
    }


    public void AssertSubledgerAccountRuleWithNoSector() {
      Assertion.Require(_account.Role == AccountRole.Control,
                      $"La cuenta {_account.Number} no maneja auxiliares para el sector 00.");
    }


    public void AssertSubledgerAccountRuleWithSector(Sector sector) {
      Assertion.Require(sector, nameof(sector));

      SectorRule sectorRule = _account.GetSectors(_accountingDate)
                                      .Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.RequireFail($"La cuenta {_account.Number} no maneja el sector {sector.FullName}.");

      } else {
        Assertion.Require(_account.Role == AccountRole.Sectorizada &&
                          sectorRule.SectorRole == AccountRole.Control,
            $"La cuenta {_account.Number} no requiere un auxiliar para el sector ({sector.Code}).");
      }
    }


    public void AssertNoSubledgerAccountRuleWithNoSector() {
      Assertion.Require(_account.Role == AccountRole.Detalle,
                       $"La cuenta {_account.Number} requiere un auxiliar.");
    }


    public void AssertNoSubledgerAccountRuleWithSector(Sector sector) {
      Assertion.Require(sector, nameof(sector));

      SectorRule sectorRule = _account.GetSectors(_accountingDate).Find(x => x.Sector.Equals(sector));

      if (sectorRule == null) {
        Assertion.RequireFail($"La cuenta {_account.Number} no maneja el sector {sector.FullName}.");

      } else {
        Assertion.Require(_account.Role == AccountRole.Sectorizada &&
                          sectorRule.SectorRole == AccountRole.Detalle,
                         $"La cuenta {_account.Number} maneja auxiliares para el sector ({sector.Code}).");
      }
    }


    public bool CanSkipAssertionChecking() {
      if (_account.Number == "6.05.01.02.03.03" &&
          _accountingDate == new DateTime(2023, 01, 01)) {
        return true;
      }
      return false;
    }

    #endregion Assertions

  } // class AccountAssertions

}  // namespace Empiria.FinancialAccounting
