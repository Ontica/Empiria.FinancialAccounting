/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Cash Ledger                                Component : Domain Layer                            *
*  Assembly : FinancialAccounting.CashLedger.dll         Pattern   : Information holder                      *
*  Type     : CashEntry                                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds data for a cash transaction entry.                                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.CashLedger {

  /// <summary>Holds data for a cash transaction entry.</summary>
  internal class CashEntry {

    #region Properties

    [DataField("ID_MOVIMIENTO")]
    public long Id {
      get; private set;
    }

    [DataField("ID_TRANSACCION")]
    public long VoucherId {
      get; private set;
    }

    [DataField("ID_CUENTA", ConvertFrom = typeof(long))]
    public LedgerAccount LedgerAccount {
      get; private set;
    }


    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; private set;
    }


    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(long))]
    public SubledgerAccount SubledgerAccount {
      get; private set;
    }


    [DataField("ID_MOVIMIENTO_REFERENCIA", ConvertFrom = typeof(long))]
    public int CashAccountId {
      get; private set;
    }


    [DataField("ID_AREA_RESPONSABILIDAD", ConvertFrom = typeof(long))]
    public FunctionalArea ResponsibilityArea {
      get; private set;
    }


    [DataField("CLAVE_PRESUPUESTAL")]
    public string BudgetCode {
      get; private set;
    }


    [DataField("CLAVE_DISPONIBILIDAD")]
    private string _claveDisponibilidad;

    public EventType EventType {
      get {
        if (String.IsNullOrWhiteSpace(_claveDisponibilidad) ||
            Convert.ToInt32(_claveDisponibilidad) == 0) {
          return EventType.Empty;
        }
        return EventType.Parse(Convert.ToInt32(_claveDisponibilidad));
      }
      private set {
        _claveDisponibilidad = value.Id.ToString();
      }
    }


    [DataField("NUMERO_VERIFICACION")]
    public string VerificationNumber {
      get; private set;
    }


    [DataField("TIPO_MOVIMIENTO", Default = VoucherEntryType.Debit)]
    public VoucherEntryType VoucherEntryType {
      get; private set;
    }


    [DataField("FECHA_MOVIMIENTO", Default = "ExecutionServer.DateMinValue")]
    public DateTime Date {
      get; private set;
    }


    [DataField("CONCEPTO_MOVIMIENTO")]
    public string Description {
      get; private set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(long))]
    public Currency Currency {
      get; private set;
    }


    [DataField("MONTO")]
    public decimal Amount {
      get; private set;
    }


    [DataField("MONTO_MONEDA_BASE")]
    public decimal BaseCurrencyAmount {
      get; private set;
    }

    public decimal Debit {
      get {
        return this.VoucherEntryType == VoucherEntryType.Debit ? this.Amount : 0m;
      }
    }


    public decimal Credit {
      get {
        return this.VoucherEntryType == VoucherEntryType.Credit ? this.Amount : 0m;
      }
    }


    public decimal ExchangeRate {
      get {
        return Math.Round(BaseCurrencyAmount / Amount, 6);
      }
    }

    public bool HasSubledgerAccount {
      get {
        return !SubledgerAccount.IsEmptyInstance;
      }
    }

    #endregion Properties

  }  // class CashEntry

}  // namespace Empiria.FinancialAccounting.CashLedger
