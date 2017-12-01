# Satku CLI tool

This tool was needed when the built in tooling for Azure Storage Tables was not that great.
Now that there is e.g. Azure Storage Explorer, this tool is half redundant. It is kept for
any special needs like uploading old data for now. Also this tool is safer as it blocks
any production data modifications, whereas a generic tool like the Storage Explorer doesn't
care.