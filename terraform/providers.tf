terraform {
  required_version = "~> 1.5"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.16"
    }
  }

  cloud {
    organization = "commitglobal"

    workspaces {
      tags = [
        "votemonitor"
      ]
    }
  }
}

provider "aws" {
  region = var.region

  default_tags {
    tags = {
      app = "votemonitor"
      env = var.env
    }
  }
}
