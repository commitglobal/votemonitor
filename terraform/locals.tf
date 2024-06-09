locals {
  namespace = "votemonitor-${var.env}"

  images = {
    api = {
      image = "commitglobal/votemonitor"
      tag   = "0.2.15"
    }

    hangfire = {
      image = "commitglobal/votemonitor-hangfire"
      tag   = "0.2.15"
    }
  }

  ecs = {
    instance_types = {
      "t3a.medium" = ""
    }
  }

  db = {
    name           = "votemonitor"
    instance_class = var.env == "production" ? "db.m7g.2xlarge" : "db.t4g.micro"
  }

  networking = {
    cidr_block = "10.0.0.0/16"

    public_subnets = [
      "10.0.1.0/24",
      "10.0.2.0/24",
      "10.0.3.0/24"
    ]

    private_subnets = [
      "10.0.4.0/24",
      "10.0.5.0/24",
      "10.0.6.0/24"
    ]
  }
}
