namespace HealthTracker.Mvc.Helpers
{
    public static class HtmlAttributesBuilder
    {
        /// <summary>
        /// Build a set of HTML attributes suitable for an integer field
        /// </summary>
        /// <param name="editable"></param>
        /// <returns></returns>
        public static Dictionary<string, object> FormControlAttributes(bool editable)
            => editable ? new() {{ "class", "form-control" }} : new() {{ "class", "form-control" }, { "disabled", "" }};

        /// <summary>
        /// Build a set of HTML attributes suitable for an integer field
        /// </summary>
        /// <param name="editable"></param>
        /// <returns></returns>
        public static Dictionary<string, object> IntegerAttributes(bool editable)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", "form-control" },
                { "type", "text" },
                { "inputmode", "numeric" },
                { "pattern", "[0-9]*" },
                { "oninput", "this.value = this.value.replace(/[^0-9]/g, '')" }
            };

            if (!editable)
            {
                attributes.Add("disabled", "");
            }

            return attributes;
        }

        /// <summary>
        /// Build a set of HTML attributes suitable for an decimal field
        /// </summary>
        /// <param name="editable"></param>
        /// <returns></returns>
        public static Dictionary<string, object> DecimalAttributes(bool editable)
        {
            var attributes = new Dictionary<string, object>
            {
                { "class", "form-control" },
                { "type", "text" },
                { "inputmode", "numeric" },
                { "pattern", @"^\d+(\.\d*)?$" },
                { "oninput", @"this.value = this.value.replace(/[^0-9.]/g, '').replace(/(\..*)\./g, '$1')" }
            };

            if (!editable)
            {
                attributes.Add("disabled", "");
            }

            return attributes;
        }
    }
}