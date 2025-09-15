/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Helper methods                          *
*  Type     : VoucherHelper                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Helper methods for an accounting voucher.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Linq;

using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Helper methods for an accounting voucher.</summary>
  internal class VoucherHelper {

    private Voucher _voucher;

    internal VoucherHelper(Voucher voucher) {
      _voucher = voucher;
    }

    #region Propreties

    internal Participant AccountingManager {
      get {
        var workgroup = VoucherWorkgroup.Parse("Accounting.Manager");

        return workgroup.Members[0];
      }
    }

    internal VoucherActions Actions {
      get {
        if (_voucher.IsClosed) {
          return new VoucherActions {
            ChangeConcept = IsAccountingDateOpened,
            CloneVoucher = true
          };
        }

        bool isAssignedToCurrentUser = _voucher.ElaboratedBy.Equals(Participant.Current);
        bool wasSentToAnotherUser = !_voucher.AuthorizedBy.IsEmptyInstance &&
                                    !_voucher.AuthorizedBy.Equals(Participant.Current);

        if (!IsValid) {
          return new VoucherActions {
            DeleteVoucher = true,
            EditVoucher = true,
            ReviewVoucher = true
          };
        }

        if (CanBeClosedBy(Participant.Current)) {
          return new VoucherActions {
            CloneVoucher = true,
            DeleteVoucher = true,
            EditVoucher = true,
            SendToLedger = true
          };

        } else if (!wasSentToAnotherUser) {
          return new VoucherActions {
            CloneVoucher = true,
            DeleteVoucher = true,
            EditVoucher = true,
            SendToSupervisor = true
          };

        } else {  // wasSentToAnotherUser
          return new VoucherActions {
            CloneVoucher = true,
          };
        }
      }
    }


    internal bool CanCloseWithProtectedAccounts {
      get {
        return ExecutionServer.CurrentPrincipal.IsInRole("administrador-operativo") ||
               ExecutionServer.CurrentPrincipal.HasPermission("registro-manual-cuentas-protegidas");
      }
    }


    internal bool CurrentUserIsSupervisor {
      get {
        var workgroup = VoucherWorkgroup.Parse("Vouchers.Authorization.Group");

        return workgroup.Members.Contains(x => x.Id == ExecutionServer.CurrentUserId);

      }
    }

    internal bool HasProtectedAccounts {
      get {
        if (_voucher.Entries.Count == 0) {
          return false;
        }

        const string PROTECTED_ACCOUNTS_PREFIX = "4.02.03.01";

        return _voucher.Entries.Contains(x => x.LedgerAccount.Number.StartsWith(PROTECTED_ACCOUNTS_PREFIX));
      }
    }


    internal bool IsAccountingDateOpened {
      get {
        return _voucher.Ledger.IsAccountingDateOpened(_voucher.AccountingDate);
      }
    }


    public bool IsValid {
      get {
        if (_voucher.IsClosed) {
          return true;
        }

        return GetValidationResult(false).Count == 0;
      }
    }

    #endregion Propreties

    #region Methods

    internal bool CanBeClosedBy(Participant participant) {
      if (_voucher.IsClosed) {
        return false;
      }

      if (IsSupervisor(participant)) {
        return true;
      }

      if (HasProtectedAccounts && !CanCloseWithProtectedAccounts) {
        return false;
      }

      if (IsAccountingDateOpened) {
        return true;
      }

      return false;
    }


    internal void EnsureAllEntriesAreValid() {
      _voucher.RefreshEntries();

      var entries = _voucher.Entries;

      Assertion.Require(entries.All(x => x.LedgerAccount.Ledger.Equals(_voucher.Ledger)),
             $"Cuando menos un movimiento tiene una cuenta que pertenece a otro mayor. (Póliza {_voucher.Id}).");

      Assertion.Require(entries.All(x => x.Amount > 0),
          $"Cuando menos un movimiento tiene un importe negativo o igual a cero. (Póliza {_voucher.Id}).");

      Assertion.Require(entries.All(x => x.BaseCurrencyAmount > 0),
          $"Cuando menos un movimiento tiene un importe negativo o igual a cero" +
          $"para la moneda origen (Póliza {_voucher.Id}).");

      Assertion.Require(entries.All(x => x.SubledgerAccount.IsEmptyInstance ||
                        (x.SubledgerAccount.BelongsTo(_voucher.Ledger) && !x.SubledgerAccount.Suspended)),
          $"Cuando menos un movimiento tiene un auxiliar que pertenece " +
          $"a otra contabilidad o está suspendido. (Póliza {_voucher.Id}).");
    }


    internal FixedList<string> GetValidationResult(bool fullValidation) {
      _voucher.RefreshEntries();

      string exception = string.Empty;

      string UID_POLIZA_CARGA_SALDOS_INICIALES = "c94bfd2b-84a7-4807-99fc-d4cb23cd43e3";
      string UID_POLIZA_EFECTOS_INICIALES_ADOPCION_NORMA = "e05e39ed-e744-43e1-b7b1-8e7b6c4e9895";

      if (_voucher.AccountingDate.Date == new DateTime(2022, 1, 1) &&
          _voucher.VoucherType.UID != UID_POLIZA_CARGA_SALDOS_INICIALES) {
        exception = "El primero de enero de 2022 sólo permite el registro de pólizas de 'Carga de saldos iniciales'.";
      }

      if (_voucher.AccountingDate.Date != new DateTime(2022, 1, 1) &&
          _voucher.VoucherType.UID == UID_POLIZA_CARGA_SALDOS_INICIALES) {
        exception = "Las pólizas de 'Carga de saldos iniciales' deben registrarse con fecha primero de enero de 2022.";
      }


      if (_voucher.AccountingDate.Date == new DateTime(2022, 1, 2) &&
          _voucher.VoucherType.UID != UID_POLIZA_EFECTOS_INICIALES_ADOPCION_NORMA) {
        exception = "El 2 de enero de 2022 sólo permite el registro de pólizas de 'Efectos iniciales de adopción de Norma'.";
      }

      if (_voucher.AccountingDate.Date != new DateTime(2022, 1, 2) &&
          _voucher.VoucherType.UID == UID_POLIZA_EFECTOS_INICIALES_ADOPCION_NORMA) {
        exception = "Las pólizas de 'Efectos iniciales de adopción de Norma' deben registrarse con fecha 2 de enero de 2022.";
      }

      if (HasProtectedAccounts && !CanCloseWithProtectedAccounts) {
        exception = "La persona usuaria no tiene permisos para enviar al diario pólizas con cuentas protegidas.";
      }

      var validator = new VoucherValidator(_voucher.Ledger, _voucher.AccountingDate);

      var list = validator.Validate(_voucher.Entries, fullValidation).ToList();

      if (exception.Length != 0) {
        list.Insert(0, exception);
      }

      return list.ToFixedList();
    }


    internal bool IsSupervisor(Participant participant) {
      var workgroup = VoucherWorkgroup.Parse("Vouchers.Authorization.Group");

      return workgroup.Members.Contains(participant);
    }


    internal FixedList<SubledgerAccount> SearchSubledgerAccountsForEdition(LedgerAccount account, string keywords) {
      Assertion.Require(_voucher.IsOpened, "No hay cuentas auxiliares para edición porque la póliza ya está cerrada.");

      Assertion.Require(account.Ledger.Equals(_voucher.Ledger), "Account does not belong to voucher ledger.");

      var historic = account.GetHistoric(_voucher.AccountingDate);

      Assertion.Require(historic.Role == AccountRole.Control || historic.Role == AccountRole.Sectorizada,
                       "The account role is not 'Control', so there are not subledger accounts to return.");

      return VoucherData.SearchSubledgerAccountsForVoucherEdition(_voucher, keywords);
    }

    #endregion Methods

  }  // class VoucherHelper

}  // namespace Empiria.FinancialAccounting.Vouchers
