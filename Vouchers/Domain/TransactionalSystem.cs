/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria General Object                  *
*  Type     : TransactionalSystem                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the transactional system that sends transactions or accountable information.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Vouchers.Data;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Describes the transactional system that sends transactions or accountable information.</summary>
  public class TransactionalSystem : GeneralObject {

    private Lazy<FixedList<TransactionalSystemRule>> _rules;

    #region Constructors and parsers

    protected TransactionalSystem() {
      // Required by Empiria Framework.
    }

    static public TransactionalSystem Parse(int id) {
      return BaseObject.ParseId<TransactionalSystem>(id);
    }


    static public TransactionalSystem Parse(string uid) {
      return BaseObject.ParseKey<TransactionalSystem>(uid);
    }


    static public FixedList<TransactionalSystem> GetList() {
      return BaseObject.GetList<TransactionalSystem>(string.Empty, "ObjectName")
                       .ToFixedList();
    }

    static public TransactionalSystem Get(Predicate<TransactionalSystem> match) {
      FixedList<TransactionalSystem> list = GetList();

      return list.Find(match);
    }

    static public TransactionalSystem Empty => BaseObject.ParseEmpty<TransactionalSystem>();

    protected override void OnInitialize() {
      base.OnLoad();
      LoadRules();
    }

    private void LoadRules() {
      _rules = new Lazy<FixedList<TransactionalSystemRule>>(() => VoucherData.GetTransactionalSystemRules(this));
    }

    #endregion Constructors and parsers

    #region Properties

    public FixedList<TransactionalSystemRule> Rules {
      get {
        return _rules.Value;
      }
    }


    public int SourceSystemId {
      get {
        return base.ExtendedDataField.Get<int>("sourceSystemId", -1);
      }
    }

    #endregion Properties

  } // class TransactionalSystem

}  // namespace Empiria.FinancialAccounting.Vouchers
