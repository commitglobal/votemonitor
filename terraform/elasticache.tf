resource "aws_elasticache_cluster" "main" {
  cluster_id           = local.namespace
  engine               = "valkey"
  node_type            = "cache.t4g.micro"
  num_cache_nodes      = 1
  parameter_group_name = "default.valkey9"
  engine_version       = "9.0"
  port                 = 6379
  subnet_group_name    = aws_elasticache_subnet_group.elasticache_subnet_group.name
  security_group_ids   = [aws_security_group.elasticache.id]
}

resource "aws_security_group" "elasticache" {
  name        = "${local.namespace}-elasticache"
  description = "Inbound - Security Group attached to the ElastiCache Cluster"
  vpc_id      = aws_vpc.main.id

  ingress {
    from_port       = 6379
    to_port         = 6379
    protocol        = "tcp"
    security_groups = [aws_security_group.bastion.id, aws_security_group.ecs.id]
  }
}
