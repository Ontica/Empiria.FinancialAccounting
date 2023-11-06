/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Lists                             Component : Use cases Layer                         *
*  Assembly : Empiria.FinancialAccounting.dll            Pattern   : Use case interactor class               *
*  Type     : AccountsListsUseCases                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases for accounts lists.                                                                  *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.AccountsLists.SpecialCases;
using Empiria.FinancialAccounting.AccountsLists.Adapters;
using static Empiria.FinancialAccounting.AccountsLists.Adapters.DepreciacionActivoFijoListItemDto;

namespace Empiria.FinancialAccounting.AccountsLists.UseCases {

  /// <summary>Use cases for accounts lists.</summary>
  public class AccountsListsUseCases : UseCase {

    #region Constructors and parsers

    protected AccountsListsUseCases() {
      // no-op
    }


    static public AccountsListsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<AccountsListsUseCases>();
    }

    #endregion Constructors and parsers

    #region Query Use cases

    public AccountsListDto GetEditableAccountsList(string accountsListUID, string keywords) {
      var list = AccountsList.Parse(accountsListUID);

      return AccountsListMapper.Map(list, keywords);
    }


    public FixedList<NamedEntityDto> GetAccountsListsForEdition() {
      FixedList<AccountsList> accountsLists = AccountsList.GetList()
                                                          .FindAll(x => x.IsEditable);

      accountsLists = base.RestrictUserDataAccessTo(accountsLists);

      return accountsLists.MapToNamedEntityList();
    }

    #endregion Query Use cases

    #region Use cases Conciliación de derivados

    public ConciliacionDerivadosListItemDto AddConciliacionDerivadosListItem(ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = ConciliacionDerivadosList.Parse();

      ConciliacionDerivadosListItem item = list.AddItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }


    public void RemoveConciliacionDerivadosListItem(ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = ConciliacionDerivadosList.Parse();

      ConciliacionDerivadosListItem item = list.RemoveItem(fields);

      item.Save();
    }


    public ConciliacionDerivadosListItemDto UpdateConciliacionDerivadosListItem(ConciliacionDerivadosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = ConciliacionDerivadosList.Parse();

      ConciliacionDerivadosListItem item = list.UpdateItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }

    #endregion Use cases Conciliación de derivados

    #region Use cases Depreciación Activo Fijo

    public DepreciacionActivoFijoListItemDto AddDepreciacionActivoFijoListItem(DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = DepreciacionActivoFijoList.Parse();

      DepreciacionActivoFijoListItem item = list.AddItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }


    public void RemoveDepreciacionActivoFijoListItem(DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = DepreciacionActivoFijoList.Parse();

      DepreciacionActivoFijoListItem item = list.RemoveItem(fields);

      item.Save();
    }


    public DepreciacionActivoFijoListItemDto UpdateDepreciacionActivoFijoListItem(DepreciacionActivoFijoListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = DepreciacionActivoFijoList.Parse();

      DepreciacionActivoFijoListItem item = list.UpdateItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }

    #endregion Use cases Depreciación Activo Fijo

    #region Use cases Derrama SWAPS cobertura

    public SwapsCoberturaListItemDto AddSwapsCoberturaListItem(SwapsCoberturaListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = SwapsCoberturaList.Parse();

      SwapsCoberturaListItem item = list.AddItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }


    public FixedList<string> SwapsCoberturaClassifications() {
      var list = SwapsCoberturaList.Parse();

      return list.GetClassificationValues();
    }


    public void RemoveSwapsCoberturaListItem(SwapsCoberturaListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = SwapsCoberturaList.Parse();

      SwapsCoberturaListItem item = list.RemoveItem(fields);

      item.Save();
    }


    public SwapsCoberturaListItemDto UpdateSwapsCoberturaListItem(SwapsCoberturaListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = SwapsCoberturaList.Parse();

      SwapsCoberturaListItem item = list.UpdateItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }

    #endregion Use cases Derrama SWAPS cobertura

    #region Use cases Préstamos interbancarios

    public PrestamosInterbancariosListItemDto AddPrestamoInterbancarioListItem(PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = PrestamosInterbancariosList.Parse();

      PrestamosInterbancariosListItem item = list.AddItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }


    public FixedList<Prestamo> PrestamosList() {
      var list = PrestamosInterbancariosList.Parse();

      return list.GetPrestamos();
    }


    public void RemovePrestamoInterbancarioListItem(PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = PrestamosInterbancariosList.Parse();

      PrestamosInterbancariosListItem item = list.RemoveItem(fields);

      item.Save();
    }


    public PrestamosInterbancariosListItemDto UpdatePrestamoInterbancarioListItem(PrestamosInterbancariosListItemFields fields) {
      Assertion.Require(fields, nameof(fields));

      fields.EnsureValid();

      var list = PrestamosInterbancariosList.Parse();

      PrestamosInterbancariosListItem item = list.UpdateItem(fields);

      item.Save();

      return AccountsListMapper.MapEntry(item);
    }

    #endregion Use cases Préstamos interbancarios

    public void SetKeywords() {
      var list1 = SwapsCoberturaList.Parse();

      foreach (var item in list1.GetItems()) {
        item.Save();
      }

      var list2 = ConciliacionDerivadosList.Parse();

      foreach (var item in list2.GetItems()) {
        item.Save();
      }

      var list3 = DepreciacionActivoFijoList.Parse();

      foreach (var item in list3.GetItems()) {
        item.Save();
      }

      var list4 = PrestamosInterbancariosList.Parse();

      foreach (var item in list4.GetItems()) {
        item.Save();
      }

    }

  } // class AccountsListsUseCases

} // Empiria.FinancialAccounting.UseCases
