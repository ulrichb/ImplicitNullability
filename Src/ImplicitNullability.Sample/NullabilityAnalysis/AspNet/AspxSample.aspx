<%@ Page Language="C#" %>
<%@ Import Namespace="System" %>
<%@ Import Namespace="ImplicitNullability.Sample" %>
<%@ Import Namespace="JetBrains.Annotations" %>

<script runat="server">

    [CanBeNull]
    private readonly string _nullString = null;

    private readonly int? _nullableInt = null;

    private string TestMethod(string a)
    {
        ReSharper.TestValueAnalysis(a, a == null /*Expect:ConditionIsAlwaysTrueOrFalse*/);
        return "";
    }

    private string TestNullableParametersMethod([CanBeNull] string a, int? b, string c = null)
    {
        return "";
    }

</script>

<!DOCTYPE html>
<html>
<head>
    <title></title>
</head>
<body>
<%-- Render method --%>
<%= null %>
<%= _nullString %>
<%= _nullableInt %>

<%= TestMethod(null /*Expect:AssignNullToNotNullAttribute*/) %>
<%= TestNullableParametersMethod(null, null, null) %>

<%-- Data binding --%>
<%# null %>
<%# Bind(null, null) %> <%-- this handled specially by R# (AspBindMethod declared element)--%>

<%-- Note that the following warnings need external annotations for TemplateControl.Eval() --%>
<%# Eval(null /*Expect:AssignNullToNotNullAttribute*/) %>
<%# Eval(null /*Expect:AssignNullToNotNullAttribute*/, null /*Expect:AssignNullToNotNullAttribute*/) %>
</body>
</html>