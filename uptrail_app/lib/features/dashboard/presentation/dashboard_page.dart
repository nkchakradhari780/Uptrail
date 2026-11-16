import 'package:flutter/material.dart';
import 'package:flutter/cupertino.dart';

import '../../../app/theme/app_colors.dart';

class DashboardPage extends StatelessWidget{
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Employee Status'),
      ),
      body: Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
            children: [
              const Icon(
                Icons.rocket_launch_rounded,
                size: 64,
                color: AppColors.primary,
              ),
              const SizedBox(height: 16),
              const Text(
                'Yoour journey starts here.',
                style: TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8,),
              Text(
                'Welcome to your employee workspace.',
                  style: TextStyle(
                    color: AppColors.textSecondary,
                  ),
              ),
            ],
        ),
      ),
    );
  }
}