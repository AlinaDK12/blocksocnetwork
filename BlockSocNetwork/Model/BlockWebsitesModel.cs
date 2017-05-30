namespace BlockSocNetwork
{
    class BlockWebsitesModel
    {
        public Website[] Websites { get; set; }       
    }

     class Website
    {
        public string Name { get; set; }
        public string[] IP { get; set; }
        public string[] IPAddressStart { get; set; }
        public string[] IPAddressEnd { get; set; }
        public bool IsSelected { get; set; }
    }
}
