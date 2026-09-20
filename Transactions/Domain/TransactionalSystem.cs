/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Transactions Services            Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Transactions.dll       Pattern   : Common storage type                     *
*  Type     : TransactionalSystem                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Describes the transactional system that sends transactions or accountable information.         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.FinancialAccounting.Transactions.Data;

namespace Empiria.FinancialAccounting.Transactions {

  /// <summary>Describes the transactional system that sends transactions or accountable information.</summary>
  public class TransactionalSystem : CommonStorage {

    private Lazy<FixedList<TransactionalSystemRule>> _rules;

    #region Constructors and parsers

    protected TransactionalSystem() {
      // Required by Empiria Framework.
    }

    static public TransactionalSystem Parse(int id) => ParseId<TransactionalSystem>(id);

    static public TransactionalSystem Parse(string uid) => ParseKey<TransactionalSystem>(uid);

    static public TransactionalSystem ParseWithCode(string code) => ParseWithCode<TransactionalSystem>(code);

    static public FixedList<TransactionalSystem> GetList() => GetStorageObjects<TransactionalSystem>();

    static public TransactionalSystem Get(Predicate<TransactionalSystem> match) {
      FixedList<TransactionalSystem> list = GetList();

      return list.Find(match);
    }

    static public TransactionalSystem Empty => ParseEmpty<TransactionalSystem>();

    protected override void OnLoad() {
      LoadRules();
    }

    private void LoadRules() {
      _rules = new Lazy<FixedList<TransactionalSystemRule>>(() =>
                          TransactionalSystemData.GetTransactionalSystemRules(this));
    }

    #endregion Constructors and parsers

    #region Properties

    public new string Code {
      get {
        return base.Code;
      }
    }


    public int ExternalSourceSystemId {
      get {
        return ExtData.Get("externalSourceSystemId", -1);
      }
    }


    public FixedList<TransactionalSystemRule> Rules {
      get {
        return _rules.Value;
      }
    }

    #endregion Properties

  } // class TransactionalSystem

}  // namespace Empiria.FinancialAccounting.Transactions
