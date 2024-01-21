namespace RestAPI.Responses {
    internal class Default {
        public string message { get; set; }
        public Default(string message) {
            this.message = message;
        }
        public string ToJson() {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
