/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Reports                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialReports.dll   Pattern   : Dynamic Data Transfer Object            *
*  Type     : DynamicFinancialReportEntryDto             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Dynamic data transfer object for data structures that contain variable totals fields.          *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System.Collections.Generic;
using System.Dynamic;

namespace Empiria.FinancialAccounting.FinancialReports.Adapters {

  /// <summary>Dynamic data transfer object for data structures that contain variable totals fields.</summary>
  public abstract class DynamicFinancialReportEntryDto : DynamicObject {

    private readonly IDictionary<string, object> _fields = new Dictionary<string, object>();

    public override IEnumerable<string> GetDynamicMemberNames() {
      return this._fields.Keys;
    }

    public decimal GetTotalField(FinancialReportTotalField field) {
      var fieldName = field.ToString();

      if (_fields.ContainsKey(fieldName)) {
        return (decimal) _fields[fieldName];
      } else {
        return 0;
      }
    }


    public void SetTotalField(FinancialReportTotalField field, decimal value) {
      var fieldName = field.ToString();

      if (_fields.ContainsKey(fieldName)) {
        _fields[fieldName] = value;
      } else {
        _fields.Add(fieldName, value);
      }
    }


    public override bool TryGetMember(GetMemberBinder binder, out object result) {
      string fieldName = binder.Name;

      if (_fields.ContainsKey(fieldName)) {
        result = _fields[fieldName];
      } else {
        result = null;
      }

      return result != null;
    }


    public override bool TrySetMember(SetMemberBinder binder, object value) {
      string fieldName = binder.Name;

      if (_fields.ContainsKey(fieldName)) {
        _fields[fieldName] = value;
      } else {
        _fields.Add(fieldName, value);
      }

      return true;
    }

  }  // class DynamicFinancialReportEntryDto

} // namespace Empiria.FinancialAccounting.FinancialReports.Adapters
