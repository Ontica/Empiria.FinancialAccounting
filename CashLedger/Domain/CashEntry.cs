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


    [DataField("ID_AREA_RESPONSABILIDAD", ConvertFrom = typeof(long))]
    public FunctionalArea ResponsibilityArea {
      get; private set;
    }


    [DataField("CLAVE_PRESUPUESTAL")]
    public string BudgetCode {
      get; private set;
    }


    [DataField("NUMERO_VERIFICACION")]
    public string VerificationNumber {
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


    [DataField("DEBE")]
    public decimal Debit {
      get; private set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get; private set;
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

    #region Cash flow related properties

    [DataField("ID_CUENTA_FLUJO")]
    public int CashAccountId {
      get; private set;
    }


    [DataField("NUM_CONCEPTO_FLUJO")]
    public string CashAccountNo {
      get; private set;
    }


    [DataField("ID_REGLA_FLUJO")]
    public int AppliedRuleId {
      get; private set;
    }


    [DataField("TEXTO_REGLA_FLUJO")]
    public string AppliedRuleText {
      get; private set;
    }


    [DataField("ID_USUARIO_FLUJO")]
    public int CashFlowRecordedById {
      get; private set;
    }


    [DataField("FECHA_REGISTRO_FLUJO")]
    public DateTime CashFlowRecordingTime {
      get; private set;
    }


    [DataField("CONCEPTO_FLUJO_LEGADO")]
    public string CuentaSistemaLegado {
      get; private set;
    }


    public void SetCuentaSistemaLegado(string cuentaSistemaLegado) {
      Assertion.Require(cuentaSistemaLegado, nameof(CuentaSistemaLegado));
      Assertion.Require(cuentaSistemaLegado != "0", nameof(CuentaSistemaLegado));
      Assertion.Require(cuentaSistemaLegado.Length >= 4, nameof(CuentaSistemaLegado));

      CuentaSistemaLegado = cuentaSistemaLegado;
    }

    #endregion Cash flow related properties

  }  // class CashEntry

}  // namespace Empiria.FinancialAccounting.CashLedger
